﻿using System.Text.Json.Serialization;
using osuRequestor.Models;

namespace osuRequestor.Apis.OsuApi.Models
{
    public class Beatmap
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("beatmapset")]
        public BeatmapSet BeatmapSet { get; set; } = null!;

        [JsonPropertyName("version")]
        public string Version { get; set; } = null!;

        [JsonPropertyName("ar")]
        public double ApproachRate { get; set; }

        [JsonPropertyName("accuracy")]
        public double OverallDifficulty { get; set; }

        [JsonPropertyName("cs")]
        public double CircleSize { get; set; }

        [JsonPropertyName("drain")]
        public double HealthDrain { get; set; }

        [JsonPropertyName("bpm")]
        public double BeatsPerMinute { get; set; }

        [JsonPropertyName("count_circles")]
        public int Circles { get; set; }

        [JsonPropertyName("count_sliders")]
        public int Sliders { get; set; }

        [JsonPropertyName("count_spinners")]
        public int Spinners { get; set; }

        [JsonPropertyName("difficulty_rating")]
        public double StarRating { get; set; }

        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BeatmapStatus Status { get; set; }

        [JsonPropertyName("max_combo")]
        public int MaxCombo { get; set; }

        [JsonPropertyName("mode_int")]
        public Mode Mode { get; set; }

        public BeatmapModel IntoModel()
        {
            return new BeatmapModel
            {
                Id = this.Id,
                BeatmapSet = new BeatmapSetModel
                {
                    Id = this.BeatmapSet.Id,
                    Artist = this.BeatmapSet.Artist,
                    Title = this.BeatmapSet.Title,
                    CreatorId = this.BeatmapSet.CreatorId,
                },
                Version = this.Version,
                ApproachRate = this.ApproachRate,
                OverallDifficulty = this.OverallDifficulty,
                CircleSize = this.CircleSize,
                HealthDrain = this.HealthDrain,
                BeatsPerMinute = this.BeatsPerMinute,
                Circles = this.Circles,
                Sliders = this.Sliders,
                Spinners = this.Spinners,
                StarRating = this.StarRating,
                Status = this.Status,
                MaxCombo = this.MaxCombo,
                Mode = this.Mode,
            };
        }
    }
}
