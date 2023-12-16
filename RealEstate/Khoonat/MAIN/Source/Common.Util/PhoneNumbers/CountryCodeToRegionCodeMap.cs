using System.Collections.Generic;

namespace JahanJooy.Common.Util.PhoneNumbers
{
	internal class CountryCodeToRegionCodeMap
	{
		// A mapping from a country code to the region codes which denote the
		// country/region represented by that country code. In the case of multiple
		// countries sharing a calling code, such as the NANPA countries, the one
		// indicated with "isMainCountryForCode" in the metadata should be first.

		internal static IDictionary<int, IList<string>> getCountryCodeToRegionCodeMap()
		{
			// The capacity is set to 286 as there are 215 different country codes,
			// and this offers a load factor of roughly 0.75.
			IDictionary<int, IList<string>> countryCodeToRegionCodeMap =
				new Dictionary<int, IList<string>>(286);

			countryCodeToRegionCodeMap.Add(1,
			                               new List<string>
				                               {
					                               "US",
					                               "AG",
					                               "AI",
					                               "AS",
					                               "BB",
					                               "BM",
					                               "BS",
					                               "CA",
					                               "DM",
					                               "DO",
					                               "GD",
					                               "GU",
					                               "JM",
					                               "KN",
					                               "KY",
					                               "LC",
					                               "MP",
					                               "MS",
					                               "PR",
					                               "SX",
					                               "TC",
					                               "TT",
					                               "VC",
					                               "VG",
					                               "VI"
				                               });

			countryCodeToRegionCodeMap.Add(7, new List<string>(2) {"RU", "KZ"});
			countryCodeToRegionCodeMap.Add(20, new List<string>(1) {"EG"});
			countryCodeToRegionCodeMap.Add(27, new List<string>(1) {"ZA"});
			countryCodeToRegionCodeMap.Add(30, new List<string>(1) {"GR"});
			countryCodeToRegionCodeMap.Add(31, new List<string>(1) {"NL"});
			countryCodeToRegionCodeMap.Add(32, new List<string>(1) {"BE"});
			countryCodeToRegionCodeMap.Add(33, new List<string>(1) {"FR"});
			countryCodeToRegionCodeMap.Add(34, new List<string>(1) {"ES"});
			countryCodeToRegionCodeMap.Add(36, new List<string>(1) {"HU"});
			countryCodeToRegionCodeMap.Add(39, new List<string>(1) {"IT"});
			countryCodeToRegionCodeMap.Add(40, new List<string>(1) {"RO"});
			countryCodeToRegionCodeMap.Add(41, new List<string>(1) {"CH"});
			countryCodeToRegionCodeMap.Add(43, new List<string>(1) {"AT"});
			countryCodeToRegionCodeMap.Add(44, new List<string>(4) {"GB", "GG", "IM", "JE"});

			List<string> listWithRegionCode;


			listWithRegionCode = new List<string>(1) {"DK"};
			countryCodeToRegionCodeMap.Add(45, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"SE"};
			countryCodeToRegionCodeMap.Add(46, listWithRegionCode);

			listWithRegionCode = new List<string>(2) {"NO", "SJ"};
			countryCodeToRegionCodeMap.Add(47, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"PL"};
			countryCodeToRegionCodeMap.Add(48, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"DE"};
			countryCodeToRegionCodeMap.Add(49, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"PE"};
			countryCodeToRegionCodeMap.Add(51, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"MX"};
			countryCodeToRegionCodeMap.Add(52, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"CU"};
			countryCodeToRegionCodeMap.Add(53, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"AR"};
			countryCodeToRegionCodeMap.Add(54, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"BR"};
			countryCodeToRegionCodeMap.Add(55, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"CL"};
			countryCodeToRegionCodeMap.Add(56, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"CO"};
			countryCodeToRegionCodeMap.Add(57, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"VE"};
			countryCodeToRegionCodeMap.Add(58, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"MY"};
			countryCodeToRegionCodeMap.Add(60, listWithRegionCode);

			listWithRegionCode = new List<string>(3) {"AU", "CC", "CX"};
			countryCodeToRegionCodeMap.Add(61, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"ID"};
			countryCodeToRegionCodeMap.Add(62, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"PH"};
			countryCodeToRegionCodeMap.Add(63, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"NZ"};
			countryCodeToRegionCodeMap.Add(64, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"SG"};
			countryCodeToRegionCodeMap.Add(65, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"TH"};
			countryCodeToRegionCodeMap.Add(66, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"JP"};
			countryCodeToRegionCodeMap.Add(81, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"KR"};
			countryCodeToRegionCodeMap.Add(82, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"VN"};
			countryCodeToRegionCodeMap.Add(84, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"CN"};
			countryCodeToRegionCodeMap.Add(86, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"TR"};
			countryCodeToRegionCodeMap.Add(90, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"IN"};
			countryCodeToRegionCodeMap.Add(91, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"PK"};
			countryCodeToRegionCodeMap.Add(92, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"AF"};
			countryCodeToRegionCodeMap.Add(93, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"LK"};
			countryCodeToRegionCodeMap.Add(94, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"MM"};
			countryCodeToRegionCodeMap.Add(95, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"IR"};
			countryCodeToRegionCodeMap.Add(98, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"SS"};
			countryCodeToRegionCodeMap.Add(211, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"MA"};
			countryCodeToRegionCodeMap.Add(212, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"DZ"};
			countryCodeToRegionCodeMap.Add(213, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"TN"};
			countryCodeToRegionCodeMap.Add(216, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"LY"};
			countryCodeToRegionCodeMap.Add(218, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"GM"};
			countryCodeToRegionCodeMap.Add(220, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"SN"};
			countryCodeToRegionCodeMap.Add(221, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"MR"};
			countryCodeToRegionCodeMap.Add(222, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"ML"};
			countryCodeToRegionCodeMap.Add(223, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"GN"};
			countryCodeToRegionCodeMap.Add(224, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"CI"};
			countryCodeToRegionCodeMap.Add(225, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"BF"};
			countryCodeToRegionCodeMap.Add(226, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"NE"};
			countryCodeToRegionCodeMap.Add(227, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("TG");
			countryCodeToRegionCodeMap.Add(228, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("BJ");
			countryCodeToRegionCodeMap.Add(229, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("MU");
			countryCodeToRegionCodeMap.Add(230, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("LR");
			countryCodeToRegionCodeMap.Add(231, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("SL");
			countryCodeToRegionCodeMap.Add(232, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("GH");
			countryCodeToRegionCodeMap.Add(233, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("NG");
			countryCodeToRegionCodeMap.Add(234, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"TD"};
			countryCodeToRegionCodeMap.Add(235, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"CF"};
			countryCodeToRegionCodeMap.Add(236, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"CM"};
			countryCodeToRegionCodeMap.Add(237, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"CV"};
			countryCodeToRegionCodeMap.Add(238, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"ST"};
			countryCodeToRegionCodeMap.Add(239, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("GQ");
			countryCodeToRegionCodeMap.Add(240, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("GA");
			countryCodeToRegionCodeMap.Add(241, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"CG"};
			countryCodeToRegionCodeMap.Add(242, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"CD"};
			countryCodeToRegionCodeMap.Add(243, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"AO"};
			countryCodeToRegionCodeMap.Add(244, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"GW"};
			countryCodeToRegionCodeMap.Add(245, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"IO"};
			countryCodeToRegionCodeMap.Add(246, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"AC"};
			countryCodeToRegionCodeMap.Add(247, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"SC"};
			countryCodeToRegionCodeMap.Add(248, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"SD"};
			countryCodeToRegionCodeMap.Add(249, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"RW"};
			countryCodeToRegionCodeMap.Add(250, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"ET"};
			countryCodeToRegionCodeMap.Add(251, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"SO"};
			countryCodeToRegionCodeMap.Add(252, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"DJ"};
			countryCodeToRegionCodeMap.Add(253, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"KE"};
			countryCodeToRegionCodeMap.Add(254, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"TZ"};
			countryCodeToRegionCodeMap.Add(255, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("UG");
			countryCodeToRegionCodeMap.Add(256, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("BI");
			countryCodeToRegionCodeMap.Add(257, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("MZ");
			countryCodeToRegionCodeMap.Add(258, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("ZM");
			countryCodeToRegionCodeMap.Add(260, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("MG");
			countryCodeToRegionCodeMap.Add(261, listWithRegionCode);

			listWithRegionCode = new List<string>(2);
			listWithRegionCode.Add("RE");
			listWithRegionCode.Add("YT");
			countryCodeToRegionCodeMap.Add(262, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("ZW");
			countryCodeToRegionCodeMap.Add(263, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("NA");
			countryCodeToRegionCodeMap.Add(264, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"MW"};
			countryCodeToRegionCodeMap.Add(265, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"LS"};
			countryCodeToRegionCodeMap.Add(266, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"BW"};
			countryCodeToRegionCodeMap.Add(267, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"SZ"};
			countryCodeToRegionCodeMap.Add(268, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"KM"};
			countryCodeToRegionCodeMap.Add(269, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"SH"};
			countryCodeToRegionCodeMap.Add(290, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"ER"};
			countryCodeToRegionCodeMap.Add(291, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"AW"};
			countryCodeToRegionCodeMap.Add(297, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"FO"};
			countryCodeToRegionCodeMap.Add(298, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"GL"};
			countryCodeToRegionCodeMap.Add(299, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("GI");
			countryCodeToRegionCodeMap.Add(350, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("PT");
			countryCodeToRegionCodeMap.Add(351, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("LU");
			countryCodeToRegionCodeMap.Add(352, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("IE");
			countryCodeToRegionCodeMap.Add(353, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("IS");
			countryCodeToRegionCodeMap.Add(354, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"AL"};
			countryCodeToRegionCodeMap.Add(355, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"MT"};
			countryCodeToRegionCodeMap.Add(356, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"CY"};
			countryCodeToRegionCodeMap.Add(357, listWithRegionCode);

			listWithRegionCode = new List<string>(2) {"FI", "AX"};
			countryCodeToRegionCodeMap.Add(358, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"BG"};
			countryCodeToRegionCodeMap.Add(359, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"LT"};
			countryCodeToRegionCodeMap.Add(370, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"LV"};
			countryCodeToRegionCodeMap.Add(371, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"EE"};
			countryCodeToRegionCodeMap.Add(372, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"MD"};
			countryCodeToRegionCodeMap.Add(373, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("AM");
			countryCodeToRegionCodeMap.Add(374, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("BY");
			countryCodeToRegionCodeMap.Add(375, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("AD");
			countryCodeToRegionCodeMap.Add(376, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("MC");
			countryCodeToRegionCodeMap.Add(377, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"SM"};
			countryCodeToRegionCodeMap.Add(378, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"VA"};
			countryCodeToRegionCodeMap.Add(379, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"UA"};
			countryCodeToRegionCodeMap.Add(380, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"RS"};
			countryCodeToRegionCodeMap.Add(381, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"ME"};
			countryCodeToRegionCodeMap.Add(382, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"HR"};
			countryCodeToRegionCodeMap.Add(385, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"SI"};
			countryCodeToRegionCodeMap.Add(386, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"BA"};
			countryCodeToRegionCodeMap.Add(387, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"MK"};
			countryCodeToRegionCodeMap.Add(389, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("CZ");
			countryCodeToRegionCodeMap.Add(420, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("SK");
			countryCodeToRegionCodeMap.Add(421, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("LI");
			countryCodeToRegionCodeMap.Add(423, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("FK");
			countryCodeToRegionCodeMap.Add(500, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("BZ");
			countryCodeToRegionCodeMap.Add(501, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("GT");
			countryCodeToRegionCodeMap.Add(502, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"SV"};
			countryCodeToRegionCodeMap.Add(503, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"HN"};
			countryCodeToRegionCodeMap.Add(504, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"NI"};
			countryCodeToRegionCodeMap.Add(505, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"CR"};
			countryCodeToRegionCodeMap.Add(506, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"PA"};
			countryCodeToRegionCodeMap.Add(507, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"PM"};
			countryCodeToRegionCodeMap.Add(508, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"HT"};
			countryCodeToRegionCodeMap.Add(509, listWithRegionCode);

			listWithRegionCode = new List<string>(3) {"GP", "BL", "MF"};
			countryCodeToRegionCodeMap.Add(590, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"BO"};
			countryCodeToRegionCodeMap.Add(591, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"GY"};
			countryCodeToRegionCodeMap.Add(592, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"EC"};
			countryCodeToRegionCodeMap.Add(593, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("GF");
			countryCodeToRegionCodeMap.Add(594, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("PY");
			countryCodeToRegionCodeMap.Add(595, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("MQ");
			countryCodeToRegionCodeMap.Add(596, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"SR"};
			countryCodeToRegionCodeMap.Add(597, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"UY"};
			countryCodeToRegionCodeMap.Add(598, listWithRegionCode);

			listWithRegionCode = new List<string>(3) {"CW", "AN", "BQ"};
			countryCodeToRegionCodeMap.Add(599, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"TL"};
			countryCodeToRegionCodeMap.Add(670, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"NF"};
			countryCodeToRegionCodeMap.Add(672, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"BN"};
			countryCodeToRegionCodeMap.Add(673, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"NR"};
			countryCodeToRegionCodeMap.Add(674, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"PG"};
			countryCodeToRegionCodeMap.Add(675, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"TO"};
			countryCodeToRegionCodeMap.Add(676, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"SB"};
			countryCodeToRegionCodeMap.Add(677, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"VU"};
			countryCodeToRegionCodeMap.Add(678, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"FJ"};
			countryCodeToRegionCodeMap.Add(679, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"PW"};
			countryCodeToRegionCodeMap.Add(680, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"WF"};
			countryCodeToRegionCodeMap.Add(681, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"CK"};
			countryCodeToRegionCodeMap.Add(682, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"NU"};
			countryCodeToRegionCodeMap.Add(683, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"WS"};
			countryCodeToRegionCodeMap.Add(685, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"KI"};
			countryCodeToRegionCodeMap.Add(686, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"NC"};
			countryCodeToRegionCodeMap.Add(687, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"TV"};
			countryCodeToRegionCodeMap.Add(688, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"PF"};
			countryCodeToRegionCodeMap.Add(689, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"TK"};
			countryCodeToRegionCodeMap.Add(690, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"FM"};
			countryCodeToRegionCodeMap.Add(691, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"MH"};
			countryCodeToRegionCodeMap.Add(692, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"001"};
			countryCodeToRegionCodeMap.Add(800, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"001"};
			countryCodeToRegionCodeMap.Add(808, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"KP"};
			countryCodeToRegionCodeMap.Add(850, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"HK"};
			countryCodeToRegionCodeMap.Add(852, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"MO"};
			countryCodeToRegionCodeMap.Add(853, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"KH"};
			countryCodeToRegionCodeMap.Add(855, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"LA"};
			countryCodeToRegionCodeMap.Add(856, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"001"};
			countryCodeToRegionCodeMap.Add(870, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"001"};
			countryCodeToRegionCodeMap.Add(878, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"BD"};
			countryCodeToRegionCodeMap.Add(880, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"001"};
			countryCodeToRegionCodeMap.Add(881, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"001"};
			countryCodeToRegionCodeMap.Add(882, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"001"};
			countryCodeToRegionCodeMap.Add(883, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"TW"};
			countryCodeToRegionCodeMap.Add(886, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"001"};
			countryCodeToRegionCodeMap.Add(888, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"MV"};
			countryCodeToRegionCodeMap.Add(960, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("LB");
			countryCodeToRegionCodeMap.Add(961, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("JO");
			countryCodeToRegionCodeMap.Add(962, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("SY");
			countryCodeToRegionCodeMap.Add(963, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("IQ");
			countryCodeToRegionCodeMap.Add(964, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("KW");
			countryCodeToRegionCodeMap.Add(965, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("SA");
			countryCodeToRegionCodeMap.Add(966, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("YE");
			countryCodeToRegionCodeMap.Add(967, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"OM"};
			countryCodeToRegionCodeMap.Add(968, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"PS"};
			countryCodeToRegionCodeMap.Add(970, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"AE"};
			countryCodeToRegionCodeMap.Add(971, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"IL"};
			countryCodeToRegionCodeMap.Add(972, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"BH"};
			countryCodeToRegionCodeMap.Add(973, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"QA"};
			countryCodeToRegionCodeMap.Add(974, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"BT"};
			countryCodeToRegionCodeMap.Add(975, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"MN"};
			countryCodeToRegionCodeMap.Add(976, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("NP");
			countryCodeToRegionCodeMap.Add(977, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("001");
			countryCodeToRegionCodeMap.Add(979, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("TJ");
			countryCodeToRegionCodeMap.Add(992, listWithRegionCode);

			listWithRegionCode = new List<string>(1);
			listWithRegionCode.Add("TM");
			countryCodeToRegionCodeMap.Add(993, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"AZ"};
			countryCodeToRegionCodeMap.Add(994, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"GE"};
			countryCodeToRegionCodeMap.Add(995, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"KG"};
			countryCodeToRegionCodeMap.Add(996, listWithRegionCode);

			listWithRegionCode = new List<string>(1) {"UZ"};
			countryCodeToRegionCodeMap.Add(998, listWithRegionCode);

			return countryCodeToRegionCodeMap;
		}

	}
}