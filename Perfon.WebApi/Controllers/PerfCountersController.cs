﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Perfon.Core;
using Perfon.Core.PerfCounterStorages;
using Perfon.Interfaces.PerfCounterStorage;
using Perfon.WebApi;

namespace Perfon.WebApi
{
    /// <summary>
    /// Web Api Controller for retrieving perf counters, available by default for PerfLib clients
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "GET")]
    [AuthPerfMonitorLibActions]
    public class PerfCountersController : ApiController
    {
        readonly string keyList = "PerfListCounters";


        private IPerfomanceCountersStorage Db
        {
            get
            {
                //How to deal with DI here??
                return (this.ControllerContext.Configuration.Properties[EnumKeyNames.PerfMonitorLib.ToString()] as PerfMonitorForWebApi).Storage;
            }
        }


        /// <summary>
        /// Get counters list
        /// </summary>
        /// <returns></returns>
        [Route("api/perfcounters")]
        public async Task<IHttpActionResult> Get()
        {
            var res = MemoryCache.Default.Get(keyList) as IEnumerable<string>;

            if (res == null || res.Count() <= 0)
            {
                // Not the best solution. Use Lazy here??
                res = await Db.GetCountersList();
                MemoryCache.Default.Add(keyList, res, new DateTimeOffset(DateTime.Now.AddSeconds(60)));
            }

            return Ok(res);
        }

        /// <summary>
        /// Get perf counter values history track
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route("api/perfcounters")]
        public async Task<IHttpActionResult> Get([FromUri]string name, [FromUri]DateTime? date = null, [FromUri]int? skip = null)
        {
            int skip2 = 0;
            if (skip.HasValue)
            {
                skip2 = skip.Value;
            }

            int expirationSec = 3;
            if (skip2 == 0)
            {
                expirationSec = 7;
            }

            string key = name + date.GetHashCode() + skip2.GetHashCode();
            var res = MemoryCache.Default.Get(key) as IEnumerable<IPerfCounterValue>;

            if (res == null || res.Count() <= 0)
            {
                // Not the best solution. Use Lazy here??
                res = await Db.QueryCounterValues(name, date, skip2);
                MemoryCache.Default.Add(key, res, new DateTimeOffset(DateTime.Now.AddSeconds(expirationSec)));
            }

            //if (res != null && skip2 > 0)
            //{
            //    res = res.Skip(skip2).ToList();
            //}

            return Ok(res);
        }

    }
}