﻿using System.Collections.Generic;
using System.Threading.Tasks;
using NovgorodBot.Models.Internal;

namespace NovgorodBot.Services
{
    public interface IDialogflowService
    {
        Task<Dialog> GetResponseAsync(Request request, IDictionary<string, string> eventParameters = null);
    }
}