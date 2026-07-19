using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace KspMissionPlanner.Planning;

public sealed class FileMissionProgramStore : IMissionProgramStore
{
    private const string FilePrefix = "mission-";
    private const string FileExtension = ".json";

    private readonly string _storageDirectory;
    private readonly JsonSerializerOptions _serializerOptions;

    public FileMissionProgramStore(string storageDirectory)
    {
        if (string.IsNullOrWhiteSpace(storageDirectory))
        {
            throw new ArgumentException("Storage directory must be provided.", nameof(storageDirectory));
        }

        _storageDirectory = storageDirectory;
        Directory.CreateDirectory(_storageDirectory);
        _serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
        };
    }

    public void Save(MissionProgram missionProgram)
    {
        if (missionProgram is null)
        {
            throw new ArgumentNullException(nameof(missionProgram));
        }

        if (string.IsNullOrWhiteSpace(missionProgram.MissionId))
        {
            missionProgram.MissionId = Guid.NewGuid().ToString("N");
        }

        missionProgram.UpdatedUtc = DateTimeOffset.UtcNow;
        var json = JsonSerializer.Serialize(missionProgram, _serializerOptions);
        File.WriteAllText(GetMissionPath(missionProgram.MissionId), json, Encoding.UTF8);
    }

    public bool TryGetById(string missionId, out MissionProgram missionProgram)
    {
        missionProgram = null!;
        if (string.IsNullOrWhiteSpace(missionId))
        {
            return false;
        }

        var path = GetMissionPath(missionId);
        if (!File.Exists(path))
        {
            return false;
        }

        var json = File.ReadAllText(path, Encoding.UTF8);
        var parsed = JsonSerializer.Deserialize<MissionProgram>(json, _serializerOptions);
        if (parsed is null)
        {
            return false;
        }

        missionProgram = parsed;
        return true;
    }

    public IReadOnlyList<MissionProgram> GetAll()
    {
        var programs = new List<MissionProgram>();
        foreach (var path in Directory.GetFiles(_storageDirectory, FilePrefix + "*" + FileExtension, SearchOption.TopDirectoryOnly))
        {
            var json = File.ReadAllText(path, Encoding.UTF8);
            var parsed = JsonSerializer.Deserialize<MissionProgram>(json, _serializerOptions);
            if (parsed is not null)
            {
                programs.Add(parsed);
            }
        }

        return programs
            .OrderByDescending(program => program.UpdatedUtc)
            .ToList();
    }

    public bool Delete(string missionId)
    {
        if (string.IsNullOrWhiteSpace(missionId))
        {
            return false;
        }

        var path = GetMissionPath(missionId);
        if (!File.Exists(path))
        {
            return false;
        }

        File.Delete(path);
        return true;
    }

    private string GetMissionPath(string missionId)
    {
        return Path.Combine(_storageDirectory, FilePrefix + SanitizeForFileName(missionId) + FileExtension);
    }

    private static string SanitizeForFileName(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var sb = new StringBuilder(value.Length);
        foreach (var ch in value)
        {
            sb.Append(Array.IndexOf(invalid, ch) >= 0 ? '_' : ch);
        }

        return sb.ToString();
    }
}