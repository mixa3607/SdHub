﻿namespace SdHub.Shared.AspErrorHandling.ModelState;

public static class ModelStateErrorHelper
{
    public static string Build(string errName, params object?[] arguments)
    {
        return errName + "$" + string.Join("&", arguments);
    }
}