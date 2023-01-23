﻿using Microsoft.AspNetCore.Mvc;

namespace Common.Extensions
{
    public static class ControllerExtension
    {
        public static string? ControllerAction<T>(this IUrlHelper urlHelper, string name, object? arg)
            where T : ControllerBase
        {
            var ct = typeof(T);
            var mi = ct.GetMethod(name);
            
            if (mi == null)
                return null;

            var controller = ct.Name.Replace("Controller", string.Empty);
            var action = urlHelper.Action(name, controller, arg);
            return action;
        }
    }
}
