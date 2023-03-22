using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

public class HexDumpConfiguration
{
    public int BytesPerLine;
    public bool ShowOffset;
    public bool Uppercase = true;
}

public class HexDump
{
    public List<byte> Data { get; set; }

    public HexDumpConfiguration Configuration { get; set; }

    public HexDump(List<byte> data)
    {
        this.Data = data;
        this.Configuration = new HexDumpConfiguration
        {
            BytesPerLine = 16,
            ShowOffset = true,
            Uppercase = true
        };
    }

    public HexDump(byte[] data)
    {
        this.Data = data.ToList();
        this.Configuration = new HexDumpConfiguration
        {
            BytesPerLine = 16,
            ShowOffset = true,
            Uppercase = true
        };
    }

    public HexDump(List<byte> data, HexDumpConfiguration configuration)
    {
        this.Data = data;
        this.Configuration = configuration;
    }

    private const int BytesPerLine = 16;

    private string DumpLine(List<byte> data, int offset)
    {
        var sb = new StringBuilder();

        if (this.Configuration.ShowOffset)
        {
            if (this.Configuration.Uppercase)
            {
                sb.Append($"{offset:X08}: ");
            }
            else
            {
                sb.Append($"{offset:x08}: ");
            }
        }

        foreach (var b in data)
        {
            if (this.Configuration.Uppercase)
            {
                sb.Append($"{b:X02} ");
            }
            else
            {
                sb.Append($"{b:x02} ");
            }
        }

        return sb.ToString();
    }

    private List<string> Dump()
    {
        List<byte[]> chunks;
        if (this.Configuration.BytesPerLine != 0)
        {
            chunks = this.Data.Chunk(this.Configuration.BytesPerLine).ToList();
        }
        else
        {
            chunks = new List<byte[]> { this.Data.ToArray() };
        }

        List<string> lines = new();

        int offset = 0;
        var line = new StringBuilder();
        foreach (var chunk in chunks)
        {
            lines.Add(DumpLine(chunk.ToList(), offset));
            offset += chunk.Length;
        }

        return lines;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var line in Dump())
        {
            sb.AppendLine(line);
        }

        return sb.ToString();
    }
}
