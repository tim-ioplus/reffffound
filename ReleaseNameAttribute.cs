using System;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
public sealed class ReleaseNameAttribute : Attribute
{
    public string Name { get; }

    public ReleaseNameAttribute(string name)
    {
        Name = name;
    }
}

