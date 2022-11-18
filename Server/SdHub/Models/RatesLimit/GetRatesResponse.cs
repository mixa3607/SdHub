namespace SdHub.Models.RatesLimit;

public class GetRatesResponse
{
    public UserPlanModel? CurrentPlan { get; set; }
    public UserPlanSpendModel? Spend { get; set; }
}