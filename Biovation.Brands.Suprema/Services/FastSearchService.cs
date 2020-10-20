using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biovation.Brands.Suprema.Model;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v1;

namespace Biovation.Brands.Suprema.Services
{
    public class FastSearchService
    {
        private Dictionary<int, FastSearch> _templates;
        private readonly AccessGroupService _accessGroupService;
        //private readonly FingerTemplateService _templateService = new FingerTemplateService();
        private readonly Semaphore _initSemaphore = new Semaphore(1, 1);

        public FastSearchService(AccessGroupService accessGroupService)
        {
            _accessGroupService = accessGroupService;
            _templates = new Dictionary<int, FastSearch>();
            //Initial();
        }


        public void Initial(bool checkExistence = false)
        {
            Logger.Log(" FastSearchService initialization started.");

            try
            {
                _initSemaphore.WaitOne(20000);
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }

            if (checkExistence)
                if (_templates.Count > 0)
                    return;


            //var userGroupToUser = _accessGroupService.GetServerSideIdentificationCacheNoTemplate();

            var accessGroups = _accessGroupService.GetAccessGroups(nestingDepthLevel: 1);


            //var allTemplates = _templateService.GetAllFingerTemplatesByFingerTemplateType(FingerTemplateType.SU384);
            //if (allTemplates.Count == 0)
            //{
            //    return;
            //}

            //var usersAccessGroups = new Dictionary<long, List<int>>();

            //for (var l = 0; l < accessGroups.Count; l++)
            //{
            //    var accessGroup = accessGroups[l];
            //    for (var k = 0; k < accessGroup.UserGroup.Count; k++)
            //    {
            //        var userGroup = accessGroup.UserGroup[k];
            //        for (var j = 0; j < userGroup.Users.Count; j++)
            //        {
            //            var userGroupMember = userGroup.Users[j];
            //            if (!usersAccessGroups.Keys.Contains(userGroupMember.UserId))
            //            {
            //                usersAccessGroups.Add(userGroupMember.UserId, new List<int>());
            //            }

            //            if (!usersAccessGroups[userGroupMember.UserId].Contains(accessGroup.Id))
            //            {
            //                usersAccessGroups[userGroupMember.UserId].Add(accessGroup.Id);
            //            }
            //        }
            //    }
            //}

            //var allTemplatesByte = new byte[allTemplates.Count][];
            //var allTemplateSize = new int[allTemplates.Count];

            var allTemplatesByte = new Dictionary<int, byte[][]>();
            var allTemplateSize = new Dictionary<int, int[]>();
            var allFingerTemplates = new Dictionary<int, List<FingerTemplate>>();

            var taskList = new List<Task>();

            foreach (var accessGroup in accessGroups)
            {
                taskList.Add(Task.Run(() =>
                {
                    var cachedList =
                        _accessGroupService.GetServerSideIdentificationCacheOfAccessGroup(accessGroup.Id, DeviceBrands.SupremaCode);
                    var userCount = cachedList.Count;

                    if (userCount == 0)
                        return;

                    if (!allTemplatesByte.Keys.Contains(accessGroup.Id))
                    {
                        allTemplatesByte.Add(accessGroup.Id, new byte[userCount][]);
                    }

                    if (!allTemplateSize.Keys.Contains(accessGroup.Id))
                    {
                        allTemplateSize.Add(accessGroup.Id, new int[userCount]);
                    }

                    if (!allFingerTemplates.Keys.Contains(accessGroup.Id))
                    {
                        allFingerTemplates.Add(accessGroup.Id, new List<FingerTemplate>());
                    }


                    for (var index = 0; index < cachedList.Count; index++)
                    {
                        try
                        {
                            var cachedObject = cachedList[index];
                            allTemplatesByte[accessGroup.Id][index] = new byte[384];
                            Array.Copy(cachedObject.FingerTemplate.Template, 0, allTemplatesByte[accessGroup.Id][index], 0,
                                384);
                            allTemplateSize[accessGroup.Id][index] = cachedObject.FingerTemplate.Size;

                            allFingerTemplates[accessGroup.Id].Add(cachedObject.FingerTemplate);
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception);
                        }
                    }
                }));

                //var index = Array.IndexOf(allTemplateSize[accessGroup.Id], 0);
                //if (allFingerTemplates[accessGroupId].All(fpt => fpt.UserId != allTemplates[i].UserId))
                //{
            }

            Task.WaitAll(taskList.ToArray());


            //int i2;

            //for (i2 = 0; i2 < allTemplates.Count; i2++)
            //{
            //    if (usersAccessGroups.ContainsKey(allTemplates[i].UserId))
            //    {
            //        var accessGroupIdsOfTemplate = usersAccessGroups[allTemplates[i].UserId];
            //        var accessGroupId = accessGroupIdsOfTemplate[0];
            //        var index = Array.IndexOf(allTemplateSize[accessGroupId], 0);
            //        allTemplatesByte[accessGroupId][index] = new byte[384];
            //        Array.Copy(allTemplates[i].Template, 0, allTemplatesByte[accessGroupId][index], 0, 384);
            //        allTemplateSize[accessGroupId][index] = allTemplates[i].Size;
            //        //if (allFingerTemplates[accessGroupId].All(fpt => fpt.UserId != allTemplates[i].UserId))
            //        //{
            //        allFingerTemplates[accessGroupId].Add(allTemplates[i]);
            //        //}
            //    }

            //    //usersAccessGroups.Add(allTemplates[i].UserId, new List<int>());
            //}

            foreach (var accessGroup in accessGroups)
            {
                if (_templates.Keys.Contains(accessGroup.Id)) continue;
                if (allFingerTemplates.ContainsKey(accessGroup.Id))
                    _templates.Add(accessGroup.Id, new FastSearch
                    {
                        TemplateByte = allTemplatesByte[accessGroup.Id]?.Where(template => template != null && Array.Exists(template, b => b != 0)).ToArray(),
                        TemplateSize = allTemplateSize[accessGroup.Id]?.Where(templateSize => templateSize != 0).ToArray(),
                        FingerTemplateList = allFingerTemplates[accessGroup.Id]
                    });
            }

            try
            {
                _initSemaphore.Release();
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }

            //_templates.TemplateByte = allTemplatesByte;
            //_templates.TemplateSize = allTemplateSize;
            //_templates.FingerTemplateList = allTemplates;

            Logger.Log(" FastSearchService initialization finished.");
        }

        public Dictionary<int, FastSearch> GetFastSearchObject()
        {
            //if (_templates.FingerTemplateList == null) Initial();
            if (_templates is null || _templates.Count == 0) Initial(true);
            return _templates;
        }

        public bool ClearFastSearchObject()
        {
            _templates = new Dictionary<int, FastSearch>();
            //_templates.TemplateSize = null;
            //_templates.FingerTemplateList = null;
            return true;
        }
    }
}
