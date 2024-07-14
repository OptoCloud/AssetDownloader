namespace AssetDownloader.HtmlScrapers.Gumroad.Types;


public class GumroadPageContext
{
    public string railsEnv { get; set; }
    public bool inMailer { get; set; }
    public string i18nLocale { get; set; }
    public string i18nDefaultLocale { get; set; }
    public string rorVersion { get; set; }
    public bool rorPro { get; set; }
    public string href { get; set; }
    public string location { get; set; }
    public string scheme { get; set; }
    public string host { get; set; }
    public object port { get; set; }
    public string pathname { get; set; }
    public object search { get; set; }
    public string httpAcceptLanguage { get; set; }
    public Design_Settings design_settings { get; set; }
    public Domain_Settings domain_settings { get; set; }
    public User_Agent_Info user_agent_info { get; set; }
    public Current_User current_user { get; set; }
    public Current_Seller current_seller { get; set; }
    public string csp_nonce { get; set; }
    public bool serverSide { get; set; }
}

public class Design_Settings
{
    public Font font { get; set; }
}

public class Font
{
    public string name { get; set; }
    public string url { get; set; }
}

public class Domain_Settings
{
    public string scheme { get; set; }
    public string app_domain { get; set; }
    public string root_domain { get; set; }
    public string short_domain { get; set; }
    public string discover_domain { get; set; }
    public string third_party_analytics_domain { get; set; }
}

public class User_Agent_Info
{
    public bool is_mobile { get; set; }
}

public class Current_User
{
    public string id { get; set; }
    public string email { get; set; }
    public string name { get; set; }
    public string avatar_url { get; set; }
    public bool confirmed { get; set; }
    public object[] team_memberships { get; set; }
    public Policies policies { get; set; }
}

public class Policies
{
    public Affiliate_Requests_Onboarding_Form affiliate_requests_onboarding_form { get; set; }
    public Direct_Affiliate direct_affiliate { get; set; }
    public Collaborator collaborator { get; set; }
    public ContextProduct product { get; set; }
    public Balance balance { get; set; }
    public Checkout_Offer_Code checkout_offer_code { get; set; }
    public Checkout_Form checkout_form { get; set; }
    public Upsell upsell { get; set; }
    public Settings_Payments_User settings_payments_user { get; set; }
    public Settings_Profile settings_profile { get; set; }
    public Settings_Third_Party_Analytics_User settings_third_party_analytics_user { get; set; }
    public Installment installment { get; set; }
    public Workflow workflow { get; set; }
}

public class Affiliate_Requests_Onboarding_Form
{
    public bool update { get; set; }
}

public class Direct_Affiliate
{
    public bool create { get; set; }
    public bool update { get; set; }
}

public class Collaborator
{
    public bool create { get; set; }
    public bool update { get; set; }
}

public class ContextProduct
{
    public bool create { get; set; }
}

public class Balance
{
    public bool index { get; set; }
}

public class Checkout_Offer_Code
{
    public bool create { get; set; }
}

public class Checkout_Form
{
    public bool update { get; set; }
}

public class Upsell
{
    public bool create { get; set; }
}

public class Settings_Payments_User
{
    public bool show { get; set; }
}

public class Settings_Profile
{
    public bool manage_social_connections { get; set; }
    public bool update { get; set; }
    public bool update_username { get; set; }
}

public class Settings_Third_Party_Analytics_User
{
    public bool update { get; set; }
}

public class Installment
{
    public bool create { get; set; }
}

public class Workflow
{
    public bool create { get; set; }
}

public class Current_Seller
{
    public string id { get; set; }
    public string email { get; set; }
    public string name { get; set; }
    public string subdomain { get; set; }
    public string avatar_url { get; set; }
    public bool is_buyer { get; set; }
    public Time_Zone time_zone { get; set; }
}

public class Time_Zone
{
    public string name { get; set; }
    public int offset { get; set; }
}
