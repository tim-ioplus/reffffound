using System;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
public sealed class ReleaseDateAttribute : Attribute
{
    public string Date { get; }

    public ReleaseDateAttribute(string date)
    {
        Date = date;
    }
}

