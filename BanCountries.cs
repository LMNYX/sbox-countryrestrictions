using Sandbox;
using System.Collections.Generic;
using System.Text.Json;

namespace BanCountries
{
	public struct UserIPData
	{
		public string ip { get; set; }
		public bool success { get; set; }
		public string type { get; set; }
		public string continent { get; set; }
		public string continent_code { get; set; }
		public string country { get; set; }
		public string country_code { get; set; }
		public string country_flag { get; set; }
		public string country_capital { get; set; }
		public string country_phone { get; set; }
		public string country_neighbours { get; set; }
		public string region { get; set; }
		public string city { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public string asn { get; set; }
		public string org { get; set; }
		public string isp { get; set; }
		public string timezone { get; set; }
		public string timezone_name { get; set; }
		public int timezone_dstOffset { get; set; }
		public int timezone_gmtOffset { get; set; }
		public string timezone_gmt { get; set; }
		public string currency { get; set; }
		public string currency_code { get; set; }
		public string currency_symbol { get; set; }
		public double currency_rates { get; set; }
		public string currency_plural { get; set; }
		public int completed_requests { get; set; }
	}

	// Use Alpha-2 ISO 3166-1 format
	public struct BanCountriesConfiguration
	{
		public static List<string> BannedCountries = new List<string> { "RU", "UA" };
	}

	public struct CountryInfo
	{
		public CountryInfo( string code, string full_name )
		{
			this.code = code;
			this.full_name = full_name;
		}
		public string code { get; set; }
		public string full_name { get; set; }
	}

	public partial class BanCountries
	{
		[ClientRpc]
		public static async void PerformCountryCheck()
		{
			var x = new Sandbox.Internal.Http( new System.Uri( "https://ipwhois.app/json/?lang=en" ) );
			var c = await x.GetStringAsync();

			UserIPData data = JsonSerializer.Deserialize<UserIPData>( c );
			ConsoleSystem.Run( "bc_internal_check", JsonSerializer.Serialize( new CountryInfo( data.country_code, data.country ) ) );
		}

		[ClientRpc]
		public static void Test(string country_name, bool _)
		{
			if(_)
			{
				ConsoleSystem.Run( "bc_internal_kickme" );
				return;
			}
			throw new System.Exception($"Your country is restricted from joining this server. ({country_name})");
			
		}

		[ServerCmd( "bc_internal_check" )]
		public async static void PerformInternalCountryCheck( string cntr )
		{
			CountryInfo CountryData = JsonSerializer.Deserialize<CountryInfo>( cntr );
			if ( ConsoleSystem.Caller.IsBot )
				return;
			if (BanCountriesConfiguration.BannedCountries.Contains(CountryData.code))
			{
				Test( To.Single( ConsoleSystem.Caller ), CountryData.full_name, false );
				Test( To.Single( ConsoleSystem.Caller ), CountryData.full_name, true );
			}
		}
		[ServerCmd("bc_internal_kickme")]
		public static void PerformRestrictedUserKick()
		{
			ConsoleSystem.Caller.Kick();
		}
	}
}
