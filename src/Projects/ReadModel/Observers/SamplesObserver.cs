﻿using System.Threading.Tasks;
using Projects.Contracts.Events;
using Projects.Infrastructure;
using Projects.ReadModel.Views;

namespace Projects.ReadModel.Observers
{
    public class SamplesObserver
    {
        private readonly IProjectionWriter<SampleView> _writer;

        public SamplesObserver(IProjectionWriter<SampleView> writer)
        {
            _writer = writer;
        }

        public async Task When(SampleStarted e)
        {
            await _writer.Add(e.Id, new SampleView
            {
                Id = e.Id,
                Name = e.Name,
            });
        }

        public async Task When(Step1Executed e)
        {
            await _writer.Update(e.Id, x =>
            {
                x.Quantity = e.Quantity;
                x.DueDate = e.DueDate;
            });
        }
    }}