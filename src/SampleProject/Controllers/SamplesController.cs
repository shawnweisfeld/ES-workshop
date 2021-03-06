﻿using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using SampleProject.Contracts.Commands;
using SampleProject.Domain.Samples;
using SampleProject.ReadModel.Providers;

namespace SampleProject.Controllers
{
    [RoutePrefix("api/samples")]
    public class SamplesController : ApiController
    {
        private readonly ISampleApplicationService _sampleApplicationService;
        private readonly ISamplesProvider _samplesProvider;

        public SamplesController(ISampleApplicationService sampleApplicationService, ISamplesProvider samplesProvider)
        {
            _sampleApplicationService = sampleApplicationService;
            _samplesProvider = samplesProvider;
        }

        [Route("")]
        [HttpGet]
        public async Task<IEnumerable> GetByName([FromUri]string name)
        {
            var samples = await _samplesProvider.Get(name);
            return samples.Select(sample => new
            {
                id = sample.Id,
                name = sample.Name,
                quantity = sample.Quantity,
                status = sample.Status.ToString()
            });
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<object> GetById(Guid id)
        {
            var sample = await _samplesProvider.GetById(id);
            return new
            {
                id = id,
                name = sample.Name,
                quantity = sample.Quantity,
                status = sample.Status.ToString()
            };
        }

        [Route("start")]
        [HttpPost]
        public HttpResponseMessage Start([FromBody]StartRequest req)
        {
            var id = _sampleApplicationService.When(new StartSample {Name = req.Name});
            return Request.CreateResponse(HttpStatusCode.Created, new {sampleId = id});
        }

        [Route("{sampleId}/step1")]
        [HttpPost]
        public void Step1(Guid sampleId, [FromBody]Step1Request req)
        {
            _sampleApplicationService.When(
                new DoStep1
                {
                    SampleId = sampleId, 
                    Quantity = req.Quantity,
                    DueDate = req.DueDate
                });
        }

        [Route("{sampleId}/approve")]
        [HttpPost]
        public void Approve(Guid sampleId)
        {
            _sampleApplicationService.When(
                new ApproveSample
                {
                    SampleId = sampleId, 
                });
        }

        [Route("{sampleId}/cancel")]
        [HttpPost]
        public void Cancel(Guid sampleId)
        {
            _sampleApplicationService.When(
                new CancelSample
                {
                    SampleId = sampleId, 
                });
        }
    }

    public class Step1Request
    {
        public int Quantity { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class StartRequest
    {
        public string Name { get; set; }
    }
}