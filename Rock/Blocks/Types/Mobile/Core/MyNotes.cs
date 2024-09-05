using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;

using Microsoft.Ajax.Utilities;

using Rock.Attribute;
using Rock.Common.Mobile.Blocks.Core.Notes;
using Rock.Data;
using Rock.Mobile;
using Rock.Model;
using Rock.Security;
using Rock.ViewModels.Blocks.Core.Notes;
using Rock.ViewModels.Utility;
using Rock.Web.Cache;

namespace Rock.Blocks.Types.Mobile.Core
{
    /// <summary>
    /// Allows the currently logged in individual to view notes created by them with the ability to manage associations and link unassociated notes.
    /// </summary>

    [DisplayName( "My Notes" )]
    [Category( "Mobile > Core" )]
    [Description( "View notes created by you with the ability to manage associations and link unassociated notes." )]
    [IconCssClass( "fa fa-list" )]
    [SupportedSiteTypes( Model.SiteType.Mobile )]

    #region Block Attributes

    [BlockTemplateField( "Note Item Template",
        Description = "The item template to use when rendering the notes.",
        TemplateBlockValueGuid = SystemGuid.DefinedValue.BLOCK_TEMPLATE_MOBILE_MY_NOTES,
        DefaultValue = "9A088BB9-C517-44C2-970B-D5CF3830F07A",
        IsRequired = true,
        Key = AttributeKey.NoteItemTemplate,
        Order = 0 )]

    [BooleanField( "Enable Swipe for Options",
        Description = "When enabled, swipe actions will be available for each note.",
        IsRequired = false,
        DefaultBooleanValue = true,
        ControlType = Field.Types.BooleanFieldType.BooleanControlType.Toggle,
        Key = AttributeKey.EnableSwipeForOptions,
        Order = 1 )]

    [NoteTypeField( "Person Note Types",
        Description = "The note types to allow selecting from when linking to a person.",
        AllowMultiple = true,
        IsRequired = false,
        Order = 2,
        EntityTypeName = "Rock.Model.Person",
        Key = AttributeKey.PersonNoteTypes )]

    [LinkedPage( "Person Profile Detail Page",
        Description = "The page to link to view a person profile when a note is associated to a person.",
        IsRequired = false,
        Key = AttributeKey.PersonProfilePage,
        Order = 3 )]

    [LinkedPage( "Reminder Detail Page",
        Description = "The page to link to view a reminder when a note is associated to a reminder.",
        IsRequired = false,
        Key = AttributeKey.ReminderDetailPage,
        Order = 4 )]

    [LinkedPage( "Add Connection Page",
        Description = "The page to link to add a connection that will be associated with the note.",
        IsRequired = false,
        Key = AttributeKey.AddConnectionPage,
        Order = 5 )]

    [LinkedPage( "Connection Detail Page",
        Description = "The page to link to view a connection.",
        IsRequired = false,
        Key = AttributeKey.ConnectionDetailPage,
        Order = 6 )]

    [BooleanField( "Group Notes by Date",
        Description = "When enabled, notes will be grouped by date.",
        IsRequired = false,
        DefaultBooleanValue = true,
        ControlType = Field.Types.BooleanFieldType.BooleanControlType.Toggle,
        Key = AttributeKey.GroupNotesByDate,
        Order = 7 )]

    #endregion

    [Rock.SystemGuid.EntityTypeGuid( Rock.SystemGuid.EntityType.MOBILE_CORE_MY_NOTES_BLOCK_TYPE )]
    [Rock.SystemGuid.BlockTypeGuid( Rock.SystemGuid.BlockType.MOBILE_CORE_MY_NOTES )]
    public class MyNotes : RockBlockType
    {
        #region Keys

        /// <summary>
        /// The attribute keys for the block.
        /// </summary>
        public static class AttributeKey
        {
            /// <summary>
            /// The note item template key.
            /// </summary>
            public const string NoteItemTemplate = "NoteItemTemplate";

            /// <summary>
            /// Enable swipe for options key.
            /// </summary>
            public const string EnableSwipeForOptions = "EnableSwipeForOptions";

            /// <summary>
            /// The note types key.
            /// </summary>
            public const string PersonNoteTypes = "PersonNoteTypes";

            /// <summary>
            /// The person profile page key.
            /// </summary>
            public const string PersonProfilePage = "PersonProfilePage";

            /// <summary>
            /// The reminder detail page key.
            /// </summary>
            public const string ReminderDetailPage = "ReminderDetailPage";

            /// <summary>
            /// The add connection page key.
            /// </summary>
            public const string AddConnectionPage = "AddConnectionPage";

            /// <summary>
            /// The view connection page key.
            /// </summary>
            public const string ConnectionDetailPage = "ConnectionDetailPage";

            /// <summary>
            /// Whether or not to group notes by date.
            /// </summary>
            public const string GroupNotesByDate = "GroupNotesByDate";
        }

        #endregion

        #region Properties

        /// <summary>
        /// The template to use for each item.
        /// </summary>
        /// <value>
        /// The XAML template to parse on the shell.
        /// </value>
        protected string NoteItemTemplate => Field.Types.BlockTemplateFieldType.GetTemplateContent( GetAttributeValue( AttributeKey.NoteItemTemplate ) );

        /// <summary>
        /// Whether or not to enable swipe for options.
        /// </summary>
        protected bool EnableSwipeForOptions => GetAttributeValue( AttributeKey.EnableSwipeForOptions ).AsBoolean();

        /// <summary>
        /// Gets the note type unique identifiers selected in the block configuration.
        /// </summary>
        protected ICollection<Guid> PersonNoteTypes => GetAttributeValue( AttributeKey.PersonNoteTypes ).SplitDelimitedValues().AsGuidList();

        /// <summary>
        /// Gets the detail page unique identifier.
        /// </summary>
        /// <value>
        /// The detail page unique identifier.
        /// </value>
        protected Guid? PersonProfilePageGuid => GetAttributeValue( AttributeKey.PersonProfilePage ).AsGuidOrNull();

        /// <summary>
        /// Gets the add connection page unique identifier.
        /// </summary>
        protected Guid? AddConnectionPageGuid => GetAttributeValue( AttributeKey.AddConnectionPage ).AsGuidOrNull();

        /// <summary>
        /// Gets the view connection page unique identifier.
        /// </summary>
        protected Guid? ViewConnectionPageGuid => GetAttributeValue( AttributeKey.ConnectionDetailPage ).AsGuidOrNull();

        /// <summary>
        /// Gets the reminder detail page unique identifier.
        /// </summary>
        protected Guid? ReminderDetailPageGuid => GetAttributeValue( AttributeKey.ReminderDetailPage ).AsGuidOrNull();

        #endregion

        #region Methods

        /// <summary>
        /// Gets the item templates for each note item.
        /// </summary>
        /// <returns></returns>
        private void PopulateNoteItemsInformation( List<NoteItemBag> notes )
        {
            // Load our common note entity types.
            var personEntityType = EntityTypeCache.Get<Person>();
            var personAliasEntityType = EntityTypeCache.Get<PersonAlias>();
            var reminderEntityType = EntityTypeCache.Get<Reminder>();
            var connectionRequestEntityType = EntityTypeCache.Get<ConnectionRequest>();

            // Load our services for each entity type.
            var personService = new PersonService( RockContext );
            var reminderTypeService = new ReminderTypeService( RockContext );
            var reminderService = new ReminderService( RockContext );
            var connectionService = new ConnectionRequestService( RockContext );
            var personAliasService = new PersonAliasService( RockContext );

            // Group all of the notes by their entity type.
            // This will allow us to load the additional entity information
            // for each note, depending on the entity type.
            var noteGroups = notes.GroupBy( n => n.NoteTypeEntityTypeId );
            var personList = new List<Person>();
            var personAliasList = new List<PersonAlias>();
            var reminderList = new List<Reminder>();
            var connectionRequestList = new List<ConnectionRequest>();

            foreach ( var grp in noteGroups )
            {
                // Extract the EntityId values into a simple list.
                var entityIds = grp.Select( n => n.EntityId ).ToList();

                // If note type is person
                if ( grp.Key == personEntityType.Id )
                {
                    personList = personService.Queryable()
                        .Where( p => entityIds.Contains( p.Id ) ).ToList();
                }
                // Reminder
                else if ( grp.Key == reminderEntityType.Id )
                {
                    reminderList = reminderService
                        .Queryable()
                        .Where( r => entityIds.Contains( r.Id ) ).ToList();

                    // Reminders are typically associated with a person alias.
                    foreach ( var reminder in reminderList )
                    {
                        if ( reminder.ReminderType.EntityTypeId == personAliasEntityType.Id )
                        {
                            var personAlias = personAliasService.Get( reminder.EntityId );

                            if ( personAlias != null )
                            {
                                personAliasList.Add( personAlias );
                            }
                        }
                    }
                }
                else if ( grp.Key == connectionRequestEntityType.Id )
                {
                    connectionRequestList = connectionService
                        .Queryable()
                        .Where( r => entityIds.Contains( r.Id ) ).ToList();
                }
            }

            // Loop through each note and populate the note items with the
            // additional entity type specific information, as well as the standard
            // note information (such as Template).
            foreach ( var note in notes )
            {
                var noteTypeEntityTypeId = NoteTypeCache.Get( note.NoteTypeId )?.EntityTypeId;
                if ( noteTypeEntityTypeId.HasValue )
                {
                    if ( note.EntityId.HasValue )
                    {
                        var entity = new EntityTypeService( RockContext ).GetEntity( noteTypeEntityTypeId.Value, note.EntityId.Value );
                        note.EntityName = entity?.ToString();
                        note.EntityGuid = entity?.Guid;
                    }

                    var entityType = EntityTypeCache.Get( noteTypeEntityTypeId.Value );
                    note.EntityTypeName = entityType?.FriendlyName;
                    note.NoteTypeEntityTypeGuid = entityType?.Guid;
                }

                if ( note.NoteTypeEntityTypeId == personEntityType.Id && note.EntityId.HasValue )
                {
                    var person = personList.FirstOrDefault( p => p.Id == note.EntityId.Value );

                    if ( person?.PhotoUrl.IsNotNullOrWhiteSpace() == true )
                    {
                        note.PhotoUrl = MobileHelper.BuildPublicApplicationRootUrl( person.PhotoUrl );
                    }
                }
                else if ( note.NoteTypeEntityTypeId == reminderEntityType.Id )
                {
                    var reminder = reminderList.FirstOrDefault( r => r.Id == note.EntityId.Value );

                    if ( reminder.ReminderType.EntityTypeId == personAliasEntityType.Id )
                    {
                        var personAlias = personAliasList.FirstOrDefault( p => p.Id == reminder.EntityId );
                        note.EntityName = personAlias?.Person.FullName;
                    }
                    else
                    {
                        var entity = new EntityTypeService( RockContext ).GetEntity( reminderEntityType.Id, reminder.EntityId );
                        note.EntityName = entity?.ToString();
                    }
                }
                else if ( note.NoteTypeEntityTypeId == connectionRequestEntityType.Id )
                {
                    var connectionRequest = connectionRequestList.FirstOrDefault( r => r.Id == note.EntityId.Value );

                    if ( connectionRequest != null )
                    {
                        note.EntityName = connectionRequest.PersonAlias.Person.FullName;
                    }
                }

                var mergeFields = RequestContext.GetCommonMergeFields();
                mergeFields.Add( "Note", new Lava.LavaDataWrapper( note ) );
                mergeFields.Add( "PersonEntityTypeId", personEntityType.Id );
                mergeFields.Add( "PersonAliasEntityTypeId", personAliasEntityType.Id );
                mergeFields.Add( "PersonAliasEntityTypeGuid", personAliasEntityType.Guid );
                mergeFields.Add( "ReminderEntityTypeId", reminderEntityType.Id );
                mergeFields.Add( "ConnectionEntityTypeId", connectionRequestEntityType.Id );
                mergeFields.Add( "PersonDetailPage", PersonProfilePageGuid );
                mergeFields.Add( "ReminderDetailPage", ReminderDetailPageGuid );
                mergeFields.Add( "ConnectionDetailPage", ViewConnectionPageGuid );
                mergeFields.Add( "AddConnectionPage", AddConnectionPageGuid );

                note.Template = NoteItemTemplate.ResolveMergeFields( mergeFields );
            }
        }

        /// <summary>
        /// Gets the notes created by the specified person.
        /// </summary>
        /// <param name="personGuid">The person who created the notes.</param>
        /// <param name="rockContext">The Rock context.</param>
        /// <param name="beforeDate">Load notes after the specified date, not inclusive.</param>
        /// <param name="filter">The filter options to use for the notes.</param>
        /// <param name="count">The number of notes to load.</param>
        /// <returns>A list of the notes returned from the query and a flag indicating whether or not the person has more notes.</returns>
        private (List<NoteItemBag> Notes, bool HasMore) GetNotesCreatedByPerson( Guid personGuid, DateTime? beforeDate, FilterOptionsBag filter, int count )
        {
            var noteService = new NoteService( RockContext );

            // Load all of the notes created by the person.
            var notesQry = noteService.Queryable()
                .OrderByDescending( n => n.CreatedDateTime )
                .Where( n => n.CreatedDateTime.HasValue
                    && n.CreatedByPersonAlias.Person.Guid == personGuid )
                .Where( n => ( beforeDate.HasValue && n.CreatedDateTime < beforeDate ) || !beforeDate.HasValue );

            // Show only notes that have an associated entity.
            if ( filter.ShowLinkedNotes && !filter.ShowStandaloneNotes )
            {
                notesQry = notesQry.Where( n => n.EntityId.HasValue );
            }

            // Show only notes that do not have an associated entity.
            if ( !filter.ShowLinkedNotes && filter.ShowStandaloneNotes )
            {
                notesQry = notesQry.Where( n => !n.EntityId.HasValue );
            }

            // Show only notes that are of the specified note types.
            if ( filter.LimitToNoteTypes != null && filter.LimitToNoteTypes.Any() )
            {
                var noteTypeIds = filter.LimitToNoteTypes.Select( nt => NoteTypeCache.Get( nt )?.Id ).Where( nt => nt.HasValue ).Select( nt => nt.Value );
                notesQry = notesQry.Where( n => noteTypeIds.Contains( n.NoteTypeId ) );
            }

            // Limit notes to a custom date range.
            if ( filter.UseCustomDateRange )
            {
                notesQry = notesQry.Where( n => n.CreatedDateTime >= filter.DateRangeStart && n.CreatedDateTime <= filter.DateRangeEnd );
            }

            // Limit notes to a specific number of days.
            else if ( filter.WithinDays > 0 )
            {
                var dateMinimum = DateTime.Today.AddDays( -filter.WithinDays );
                notesQry = notesQry.Where( n => n.CreatedDateTime >= dateMinimum );
            }

            var notes = notesQry.OrderByDescending( n => n.CreatedDateTime )
                .Take( count )
                .Select( note => new NoteItemBag
                {
                    EntityId = note.EntityId,
                    NoteText = note.Text,
                    NoteDate = note.CreatedDateTime,
                    NoteTypeName = note.NoteType.Name,
                    NoteTypeColor = note.NoteType.Color,
                    NoteTypeGuid = note.NoteType.Guid,
                    CreatedDateTime = note.CreatedDateTime.Value,
                    NoteTypeId = note.NoteTypeId,
                    NoteTypeEntityTypeId = note.NoteType.EntityTypeId,
                    Guid = note.Guid,
                    IsPrivateNote = note.IsPrivateNote,
                    IsAlert = note.IsAlert ?? false,
                    Id = note.Id,
                } ).ToList();

            // If the person has not created any notes
            // we don't need to continue.
            if ( !notes.Any() )
            {
                return (new List<NoteItemBag>(), false);
            }

            // This is the last, also the "oldest" date returned from our query.
            // We want to remove all notes created on this date to prevent returning
            // some but not all notes of a day.
            if ( notes.Count < count )
            {
                return (notes, false);
            }

            var lastSeenDate = notes.Last().CreatedDateTime.Date;
            notes.RemoveAll( n => n.CreatedDateTime.Date == lastSeenDate );

            // This catches the case in which we have removed every note (we only had one date).
            // Re-run the query but double the amount of notes we load to ensure we have enough to take.
            if ( !notes.Any() )
            {
                return GetNotesCreatedByPerson( personGuid, lastSeenDate, filter, count * 2 );
            }

            return (notes, true);
        }

        private List<NoteTypeCache> GetViewableNoteTypes()
        {
            return NoteTypeCache.All()
                .Where( nt => nt.UserSelectable )
                .Where( a => a.IsAuthorized( Authorization.VIEW, RequestContext.CurrentPerson ) )
                .ToList();
        }

        #endregion

        #region Block Actions

        /// <summary>
        /// Gets the initial data for the block.
        /// </summary>
        /// <returns></returns>
        [BlockAction]
        public BlockActionResult GetInitialData( GetMyNotesRequestBag options )
        {
            if ( RequestContext.CurrentPerson == null )
            {
                return ActionUnauthorized();
            }


            var notesBag = GetNotesCreatedByPerson( RequestContext.CurrentPerson.Guid, null, options.Filter, 50 );
            PopulateNoteItemsInformation( notesBag.Notes );

            var viewableNoteTypes = GetViewableNoteTypes().Select( nt => new
            {
                Name = nt.Name,
                Guid = nt.Guid,
                UserSelectable = nt.UserSelectable,
                IsMentionEnabled = nt.IsMentionEnabled,
                EntityTypeId = nt.EntityTypeId,
            } );

            return ActionOk( new
            {
                Notes = notesBag.Notes,
                ViewableNoteTypes = viewableNoteTypes,
                HasMore = notesBag.HasMore
            } );
        }

        /// <summary>
        /// Gets the notes created by the current person.
        /// </summary>
        /// <returns></returns>
        [BlockAction]
        public BlockActionResult GetMyNotes( GetMyNotesRequestBag options )
        {
            if ( RequestContext.CurrentPerson == null )
            {
                return ActionUnauthorized();
            }

            using ( var rockContext = new RockContext() )
            {
                var notesBag = GetNotesCreatedByPerson( RequestContext.CurrentPerson.Guid, options.BeforeDate?.Date, options.Filter, options.Count );
                PopulateNoteItemsInformation( notesBag.Notes );

                return ActionOk( new
                {
                    Notes = notesBag.Notes,
                    HasMore = notesBag.HasMore
                } );
            }
        }

        /// <summary>
        /// Deletes the note.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>A response that contains either an error or informs the client the note was deleted.</returns>
        [BlockAction]
        public BlockActionResult DeleteNote( DeleteNoteRequestBag options )
        {

            var service = new NoteService( RockContext );
            var note = service.Get( options.NoteGuid );

            if ( note == null )
            {
                return ActionNotFound();
            }

            if ( !note.IsAuthorized( Authorization.EDIT, RequestContext.CurrentPerson ) )
            {
                // Rock.Constants strings include HTML so don't use.
                return ActionForbidden( "You are not authorized to delete this note." );
            }

            if ( service.CanDeleteChildNotes( note, RequestContext.CurrentPerson, out var errorMessage ) && service.CanDelete( note, out errorMessage ) )
            {
                service.Delete( note, true );
                RockContext.SaveChanges();

                return ActionOk();
            }
            else
            {
                return ActionForbidden( errorMessage );
            }
        }

        /// <summary>
        /// Updates an existing note.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        [BlockAction]
        public BlockActionResult UpdateNote( UpdateNoteRequestBag options )
        {
            var noteService = new NoteService( RockContext );
            var note = noteService.Get( options.NoteGuid );
            var noteType = NoteTypeCache.Get( options.NoteTypeGuid );

            if ( note == null || noteType == null )
            {
                return ActionNotFound();
            }

            if ( !note.IsAuthorized( Authorization.EDIT, RequestContext.CurrentPerson ) )
            {
                return ActionForbidden( "You are not authorized to edit this note." );
            }


            note.Text = options.NoteText;
            note.IsAlert = options.IsAlert;
            note.IsPrivateNote = options.IsPrivate;
            note.NoteTypeId = noteType.Id;
            RockContext.SaveChanges();

            return ActionOk();
        }

        /// <summary>
        /// Will link a note to a person.
        /// </summary>
        /// <param name="options">The options for linking a note to a person.</param>
        /// <returns></returns>
        [BlockAction]
        public BlockActionResult LinkToPerson( LinkToPersonRequestBag options )
        {
            var noteService = new NoteService( RockContext );
            var personService = new PersonService( RockContext );

            var note = noteService.Get( options.NoteGuid );
            var person = personService.Get( options.PersonGuid );
            var noteType = NoteTypeCache.Get( options.NoteTypeGuid );

            if ( note == null || person == null || noteType == null )
            {
                return ActionNotFound();
            }

            note.EntityId = person.Id;
            note.NoteTypeId = noteType.Id;
            note.IsAlert = options.IsAlert;
            note.IsPrivateNote = options.IsPrivate;
            note.IsPinned = options.PinToTop;
            RockContext.SaveChanges();

            return ActionOk();
        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// The request bag to link a note to a person.
        /// </summary>
        public class LinkToPersonRequestBag
        {
            /// <summary>
            /// The person to link the note to.
            /// </summary>
            public Guid PersonGuid { get; set; }

            /// <summary>
            /// The note to link to the person.
            /// </summary>
            public Guid NoteGuid { get; set; }

            /// <summary>
            /// The note type to use for the note.
            /// </summary>
            public Guid NoteTypeGuid { get; set; }

            /// <summary>
            /// Whether or not this note should be marked as an alert.
            /// </summary>
            public bool IsAlert { get; set; }

            /// <summary>
            /// Whether or not this note should be pinned to the top.
            /// </summary>
            public bool PinToTop { get; set; }

            /// <summary>
            /// Whether or not this note should be marked as private.
            /// </summary>
            public bool IsPrivate { get; set; }
        }

        public class UpdateNoteRequestBag
        {
            public Guid NoteGuid { get; set; }
            public string NoteText { get; set; }
            public Guid NoteTypeGuid { get; set; }
            public bool IsAlert { get; set; }
            public bool IsPrivate { get; set; }
        }

        public class DeleteNoteRequestBag
        {
            public Guid NoteGuid { get; set; }
        }


        private class NoteItemBag
        {
            public int Id { get; set; }
            public Guid Guid { get; set; }
            public int? EntityId { get; set; }
            public Guid? EntityGuid { get; set; }
            public int NoteTypeId { get; set; }
            public int NoteTypeEntityTypeId { get; set; }
            public Guid? NoteTypeEntityTypeGuid { get; set; }
            public string EntityName { get; set; }
            public string NoteText { get; set; }
            public DateTime? NoteDate { get; set; }
            public string NoteTypeName { get; set; }
            public string AssociationTypeDescription { get; set; }
            public string AssociationTypeColor { get; set; }
            public DateTime CreatedDateTime { get; set; }
            public string PhotoUrl { get; set; }
            public string Template { get; set; }
            public string NoteTypeColor { get; set; }
            public string EntityTypeName { get; set; }
            /// <summary>
            /// Gets or sets the note type unique identifier.
            /// </summary>
            public Guid NoteTypeGuid { get; set; }

            public bool IsPrivateNote { get; set; }

            public bool IsAlert { get; set; }
        }

        /// <summary>
        /// A bag of filter options.
        /// </summary>
        public class FilterOptionsBag
        {
            /// <summary>
            /// Whether to show linked notes.
            /// </summary>
            public bool ShowLinkedNotes { get; set; }

            /// <summary>
            /// Whether to show standalone notes.
            /// </summary>
            public bool ShowStandaloneNotes { get; set; }

            /// <summary>
            /// The note types to limit to.
            /// </summary>
            public List<Guid> LimitToNoteTypes { get; set; } = new List<Guid>();

            /// <summary>
            /// The number of days to limit to.
            /// </summary>
            public int WithinDays { get; set; } = 0;

            public bool UseCustomDateRange { get; set; }

            public DateTime? DateRangeStart { get; set; }

            public DateTime? DateRangeEnd { get; set; }
        }

        public class GetMyNotesRequestBag
        {
            public int Count { get; set; } = 50;
            public DateTimeOffset? BeforeDate { get; set; }
            public FilterOptionsBag Filter { get; set; }
        }

        #endregion
    }
}
