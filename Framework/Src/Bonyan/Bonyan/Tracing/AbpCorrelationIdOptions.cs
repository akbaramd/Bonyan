﻿namespace Bonyan.Tracing;


public class AbpCorrelationIdOptions
{
    public string HttpHeaderName { get; set; } = "X-Correlation-Id";

    public bool SetResponseHeader { get; set; } = true;
}