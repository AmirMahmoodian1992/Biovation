﻿using System.Collections.Generic;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Repository;

namespace Biovation.CommonClasses.Service
{
    public class TimeZoneService
    {
        private readonly TimeZoneRepository _timeZoneRepository;

        public TimeZoneService(TimeZoneRepository timeZoneRepository)
        {
            _timeZoneRepository = timeZoneRepository;
        }

        public List<TimeZone> GetAllTimeZones()
        {
            return _timeZoneRepository.GetTimeZones();
        }

        public TimeZone GetTimeZoneById(int timeZoneId)
        {
            return _timeZoneRepository.GetTimeZone(timeZoneId);
        }

        public ResultViewModel ModifyTimeZoneById(TimeZone timeZone)
        {
            return _timeZoneRepository.ModifyTimeZone(timeZone);
        }

        public ResultViewModel DeleteTimeZoneById(int timeZoneId)
        {
            return _timeZoneRepository.DeleteTimeZone(timeZoneId);
        }
    }
}
