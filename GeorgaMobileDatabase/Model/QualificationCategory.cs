﻿using SQLite;

namespace GeorgaMobileClient.Model;

public class QualificationCategory
{
    [PrimaryKey]
    public string Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
}