using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.CommonClasses.Manager
{
    public static class Lookups
    {
        private static readonly LookupService LookupService = new LookupService();

        public static void FillValues()
        {
            Task.Run(() =>
            {
                var taskStatusesQuery = LookupService.GetLookups(lookupCategoryId: 1);
                var taskTypesQuery = LookupService.GetLookups(lookupCategoryId: 2);
                var taskItemTypesQuery = LookupService.GetLookups(lookupCategoryId: 3);
                var taskPrioritiesQuery = LookupService.GetLookups(lookupCategoryId: 4);
                var fingerIndexNamesQuery = LookupService.GetLookups(lookupCategoryId: 5);
                var deviceBrandsQuery = LookupService.GetLookups(lookupCategoryId: 6);
                var logEventsQuery = LookupService.GetLookups(lookupCategoryId: 7);
                var logSubEventsQuery = LookupService.GetLookups(lookupCategoryId: 8);
                var fingerTemplateTypeQuery = LookupService.GetLookups(lookupCategoryId: 9);
                var faceTemplateTypeQuery = LookupService.GetLookups(lookupCategoryId: 10);
                var matchingTypeQuery = LookupService.GetLookups(lookupCategoryId: 11);
                TaskStatuses = taskStatusesQuery.Result;
                TaskTypes = taskTypesQuery.Result;
                TaskItemTypes = taskItemTypesQuery.Result;
                TaskPriorities = taskPrioritiesQuery.Result;
                FingerIndexNames = fingerIndexNamesQuery.Result;
                DeviceBrands = deviceBrandsQuery.Result;
                LogSubEvents = logSubEventsQuery.Result;
                FingerTemplateType = fingerTemplateTypeQuery.Result;
                FaceTemplateType = faceTemplateTypeQuery.Result;
                LogEvents = logEventsQuery.Result;
                MatchingTypes = matchingTypeQuery.Result;

            }); 
        }

        public static List<Lookup> TaskStatuses { get; set; }
        public static List<Lookup> TaskTypes { get; set; }
        public static List<Lookup> TaskItemTypes { get; set; }
        public static List<Lookup> TaskPriorities { get; set; }
        public static List<Lookup> FingerIndexNames { get; set; }
        public static List<Lookup> DeviceBrands { get; set; }
        public static List<Lookup> LogSubEvents { get; set; }
        public static List<Lookup> FingerTemplateType { get; set; } 
        public static List<Lookup> FaceTemplateType { get; set; }
        public static List<Lookup> LogEvents { get; set; }
        public static List<Lookup> MatchingTypes { get; set; }
    }
}