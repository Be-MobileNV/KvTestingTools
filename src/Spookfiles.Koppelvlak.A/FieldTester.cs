using System.Collections.Generic;

namespace Spookfiles.Koppelvlak.A
{
    public class FieldTester
    {
        public static List<string> FieldsThatShouldBePresentInTrafficState()
        {
            var list = new List<string>();
            list.AddRange(new[]
            {
                "publication_time", "segment_id", "speed", "speed_metadata", "max_speed", "freeflow_speed", "flow",
                "max_speed_metadata", "flow_metadata",
                "density", "traveltime"
            });
            return list;
        }

        public static List<string> FieldsThatShouldBePresentInEvents()
        {
            var list = new List<string>();
            list.AddRange(new[]
            {"event_id", "event_code", "start_time", "update_time", "expected_end_time", "segment_id", "version"});
            return list;
        }

        public static List<string> FieldsThatShouldBePresentInSegmentState()
        {
            var list = new List<string>();
            list.AddRange(new[] {"segment_id", "update_time", "type", "value", "feed_id"});
            return list;
        }

        public static List<string> FieldsThatShouldBePresentInRoadSegments()
        {
            var list = new List<string>();
            list.AddRange(new[] {"lane", "max_speed", "length", "location", "id"});
            return list;
        }

        public static List<string> FieldsThatShouldBePresentInWeatherState()
        {
            var list = new List<string>();
            list.AddRange(new[] {"segment_id", "measurement_time", "weather_type", "value", "feed_id"});
            return list;
        }

        public static List<string> FieldsThatShouldBePresentInWeatherForecast()
        {
            var list = new List<string>();
            list.AddRange(new[] {"segment_id", "measurement_time", "weather_type", "value", "certainty", "feed_id"});
            return list;
        }

        public static List<string> FieldsThatShouldBePresentInVehicleState()
        {
            var list = new List<string>();
            list.AddRange(new[] { "measurement_time", "vehicle_id", "measurements" });
            return list;
        }
    }
}