﻿namespace Domain.Primitives;

public interface ISoftDeletable
{
    public bool IsDeleted { get; set; }
}