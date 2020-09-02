﻿using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Commands.Controllers.v2
{
    public class SettingController : Controller
    {
        private readonly SettingRepository _settingRepository;


        public SettingController(SettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        [HttpPost]
        [Route("ModifySetting")]
        public Task<ResultViewModel> ModifySetting(string value, string key)
        {
            return Task.Run(() => _settingRepository.ModifySetting(value, key));
        }

    }
}
