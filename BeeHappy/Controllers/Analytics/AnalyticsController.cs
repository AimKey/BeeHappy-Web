using Microsoft.AspNetCore.Mvc;
using PostHog;

namespace BeeHappy.Controllers.Analytics
{
    public class AnalyticsController : Controller
    {
        private readonly IPostHogClient _posthog;

        public AnalyticsController(IPostHogClient posthog)
        {
            _posthog = posthog;
        }

        [HttpPost]
        public IActionResult Track(string distinctId, string action)
        {
            // capture sync (SDK hỗ trợ)
            _posthog.Capture(distinctId, action, properties: new Dictionary<string, object?>
            {
                ["source"] = "backend",
                ["time"] = DateTime.UtcNow
            });

            return Ok();
        }

        public IActionResult TrackTest()
        {
            _posthog.Capture(
                    "test-user",                 // distinctId
                    "Clicked Button",            // eventName
                    new Dictionary<string, object>
                        {
                            { "buttonName", "BuyPremium" }
                        },
                    null,                        // groups
                    false                        // sendFeatureFlags
             );

            return Ok("Event sent to PostHog!");
        }
    }
}
