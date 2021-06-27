using System.Collections.Generic;
using Biovation.Domain;

namespace Biovation.Constants
{
    public class Lookups
    {
        //Todo fill values in startup
        //private readonly LookupService _lookupService;

        //public Lookups(LookupService lookupService)
        //{
        //    _lookupService = lookupService;
        //    var taskStatusesQuery = _lookupService.GetLookups(lookupCategoryId: 1);
        //    var taskTypesQuery = _lookupService.GetLookups(lookupCategoryId: 2);
        //    var taskItemTypesQuery = _lookupService.GetLookups(lookupCategoryId: 3);
        //    var taskPrioritiesQuery = _lookupService.GetLookups(lookupCategoryId: 4);
        //    var fingerIndexNamesQuery = _lookupService.GetLookups(lookupCategoryId: 5);
        //    var deviceBrandsQuery = _lookupService.GetLookups(lookupCategoryId: 6);
        //    var logEventsQuery = _lookupService.GetLookups(lookupCategoryId: 7);
        //    var logSubEventsQuery = _lookupService.GetLookups(lookupCategoryId: 8);
        //    var fingerTemplateTypeQuery = _lookupService.GetLookups(lookupCategoryId: 9);
        //    var faceTemplateTypeQuery = _lookupService.GetLookups(lookupCategoryId: 10);
        //    var matchingTypeQuery = _lookupService.GetLookups(lookupCategoryId: 11);
        //    TaskStatuses = taskStatusesQuery.Result;
        //    TaskTypes = taskTypesQuery.Result;
        //    TaskItemTypes = taskItemTypesQuery.Result;
        //    TaskPriorities = taskPrioritiesQuery.Result;
        //    FingerIndexNames = fingerIndexNamesQuery.Result;
        //    DeviceBrands = deviceBrandsQuery.Result;
        //    LogSubEvents = logSubEventsQuery.Result;
        //    FingerTemplateType = fingerTemplateTypeQuery.Result;
        //    FaceTemplateType = faceTemplateTypeQuery.Result;
        //    LogEvents = logEventsQuery.Result;
        //    MatchingTypes = matchingTypeQuery.Result;
        //}


        public List<Lookup> TaskStatuses { get; set; }
        public List<Lookup> TaskTypes { get; set; }
        public List<Lookup> TaskItemTypes { get; set; }
        public List<Lookup> TaskPriorities { get; set; }
        public List<Lookup> FingerIndexNames { get; set; }
        public List<Lookup> DeviceBrands { get; set; }
        public List<Lookup> LogSubEvents { get; set; }
        public List<Lookup> FingerTemplateType { get; set; } 
        public List<Lookup> FaceTemplateType { get; set; }
        public List<Lookup> LogEvents { get; set; }
        public List<Lookup> MatchingTypes { get; set; }
    }
}