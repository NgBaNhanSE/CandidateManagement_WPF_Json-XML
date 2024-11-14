
using System;
using System.Collections;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Globalization;
using System.Text.Json.Serialization;
using Candidate_BusinessObjects;

namespace Candidate_DAO
{
    public class CandidateProfileDAO
    {
        private readonly string _dataPath;
        private static CandidateProfileDAO instance;
        public CandidateProfileDAO()
        {
            _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JsonData");
        }
        public static CandidateProfileDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CandidateProfileDAO();
                }
                return instance;
            }
        }

        public ArrayList GetCandidateProfiles()
        {
            ArrayList candidateProfiles = new ArrayList();
            string filePath = Path.Combine(_dataPath, "CandidateProfile.json");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Cannot find file: {filePath}");
            }

            string jsonString = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new DateTimeConverter() }
            };

            var profiles = JsonSerializer.Deserialize<CandidateProfiles>(jsonString, options);

            if (profiles?.Candidates != null)
            {
                foreach (var profile in profiles.Candidates)
                {
                    candidateProfiles.Add(profile);
                }
            }
            return candidateProfiles;
        }

        public bool AddCandidateProfile(CandidateProfile candidate)
        {
            try
            {
                var candidates = GetCandidateProfiles();
                candidates.Add(candidate);
                return SaveCandidates(candidates);
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateCandidateProfile(CandidateProfile candidate)
        {
            try
            {
                var candidates = GetCandidateProfiles();
                bool found = false;

                for (int i = 0; i < candidates.Count; i++)
                {
                    var existing = candidates[i] as CandidateProfile;
                    if (existing.CandidateID == candidate.CandidateID)
                    {
                        candidates[i] = candidate;
                        found = true;
                        break;
                    }
                }

                if (!found) return false;
                return SaveCandidates(candidates);
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteCandidateProfile(string candidateId)
        {
            try
            {
                var candidates = GetCandidateProfiles();
                bool found = false;

                for (int i = candidates.Count - 1; i >= 0; i--)
                {
                    var candidate = candidates[i] as CandidateProfile;
                    if (candidate.CandidateID == candidateId)
                    {
                        candidates.RemoveAt(i);
                        found = true;
                        break;
                    }
                }

                if (!found) return false;
                return SaveCandidates(candidates);
            }
            catch
            {
                return false;
            }
        }

        private bool SaveCandidates(ArrayList candidates)
        {
            try
            {
                string filePath = Path.Combine(_dataPath, "CandidateProfile.json");
                var candidateProfiles = new CandidateProfiles
                {
                    Candidates = candidates.Cast<CandidateProfile>().ToList()
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new DateTimeConverter() }
                };
                string jsonString = JsonSerializer.Serialize(candidateProfiles, options);
                File.WriteAllText(filePath, jsonString);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public ArrayList GetCandidateProfileByNameJob(string? Name, string? jobposting)
        {
             var candidateProfiles = GetCandidateProfiles();
                ArrayList result = new ArrayList();

                foreach (CandidateProfile profile in candidateProfiles)
                {

                    if ((string.IsNullOrEmpty(Name) || profile.Fullname.Contains(Name)) &&
                        (string.IsNullOrEmpty(jobposting) || profile.PostingID.Contains(jobposting)))
                    {
                        result.Add(profile);
                    }
                }

            return result;
        }
        public CandidateProfile GetCandidateProfileByID(string id)
        {
            try
            {
                var profileList = GetCandidateProfiles();
                foreach (CandidateProfile profile in profileList)
                {
                    if (profile.CandidateID.Equals(id))
                    {
                        return profile;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
