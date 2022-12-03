using System;

namespace SdHub.Attributes;

[AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
public class AllowExpiredJwtAttribute : Attribute
{
    
}