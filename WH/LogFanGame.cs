/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 02/16/2016
 * Time: 11:16
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text.RegularExpressions;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of LogFan.
	/// </summary>
	public class LogFanGame : LogBody, ILogBody
	{
		public LogFanGame(string mess) { 
			base.Parse(mess);}

		const string regPattern1 =  @"SZ2LogFanGame_t{gameNumber=(\d+), gameIndex=(\d+), day=(\d+), month=(\d+), year=(\d+), hour=(\d+), minute=(\d+), second=(\d+), gameDuration=(\d+), clapAveragePower=(\d+), clapTime=(\d+), clapCount=(\d+), clapPerformance=(\d+), olaCount=(\d+)}";
		const string regPattern2 = @"game,(\d+),(\d+) [|] Fan-Game: { GameNumber=(\d+), GameIndex=(\d+), Date[\/]Time=((\d+)[.](\d+)[.](\d+))[-]?((\d+):(\d+):(\d+))?, Duration=(\d+), AverageClapPower=(\d+), ClapTime=(\d+), ClapCount=(\d+), ClapPerformance=(\d+), LaolaCount=(\d+) }";

	    public static bool Is(string mess) 		{ return IsPattern1T(mess) || IsPattern2T(mess) ; }
		static bool IsPattern1T(string mess) { return Regex.IsMatch(mess, regPattern1); }
		static bool IsPattern2T(string mess) { return Regex.IsMatch(mess, regPattern2); }
		protected override bool IsPattern1(string mess) { return IsPattern1T(mess); }
		protected override bool IsPattern2(string mess) { return IsPattern2T(mess); }
	
		public string gameNumber {get;set;}
		public string gameIndex{get;set;}
		public string day_string {get;set;}
		public string day_0 {get;set;}
		public string date_0 {get;set;}
		public string time_0 {get;set;}
		public string month_0 {get;set;}
		public string year_0 {get;set;}
		public string time_string{get;set;}
		public string hour{get;set;}
		public string minute{get;set;}
		public string second{get;set;}
		public string gameLength{get;set;}
		public string gameDuration{get;set;}
		public string clapAveragePower{get;set;}
		public string clapTime{get;set;}
		public string clapCount{get;set;}
		public string clapPerformance{get;set;}
		public string olaCount{get;set;}
		public int fanHeat{get;set;}

	
		public override void HandleType(WHInfo wh)
		{
			wh.FanHeat = new FanHeat(clapCount,olaCount, clapAveragePower, clapPerformance);
			fanHeat = wh.FanHeat.Heat;
		}
		
		public override bool ParsePattern1(string mess)
		{
			var parsed = false;
			System.Text.RegularExpressions.Regex reg = new Regex(regPattern1);
			{
				var match = reg.Match(mess);
				if (match.Success) 
				{
					gameNumber = match.Groups[1].Value;
					gameIndex = match.Groups[2].Value;
			
					day_0 = match.Groups[3].Value;
					month_0 = match.Groups[4].Value;
					year_0 = match.Groups[5].Value;
					date_0 = string.Format("{0}.{1}.{2}", day_0, month_0, year_0);
					
					hour = match.Groups[6].Value;
					minute = match.Groups[7].Value;
					second = match.Groups[8].Value;
					time_0 = string.Format("{0}:{1}:{2}", hour, minute, second);
					
					gameLength = match.Groups[9].Value;
					gameDuration = convertDuration(gameLength);
					
					clapAveragePower = match.Groups[10].Value;
					clapTime = match.Groups[11].Value;
					clapCount = match.Groups[12].Value;
					clapPerformance = match.Groups[13].Value;
					olaCount = match.Groups[14].Value;

					parsed = true;
				}
			}
			return parsed;
		}
		public override bool ParsePattern2(string mess)
		{
			var parsed = false;
			System.Text.RegularExpressions.Regex reg = new Regex(regPattern2);
			{
				var match = reg.Match(mess);
				if (match.Success) 
				{
					gameNumber = match.Groups[3].Value;
					gameIndex = match.Groups[4].Value;
					
					day_string = match.Groups[5].Value;
					day_0 = match.Groups[6].Value;
					month_0 = match.Groups[7].Value;
					year_0 = match.Groups[8].Value;
					date_0 = string.Format("{0}.{1}.{2}", day_0, month_0, year_0);
					
					time_string = match.Groups[9].Value;
					hour = match.Groups[10].Value;
					minute = match.Groups[11].Value;
					second = match.Groups[12].Value;
					time_0 = string.Format("{0}:{1}:{2}", hour, minute, second);
			
					gameLength = match.Groups[13].Value;
					gameDuration = convertDuration(gameLength);
					clapAveragePower = match.Groups[14].Value;
					clapTime = match.Groups[15].Value;
					clapCount = match.Groups[16].Value;
					clapPerformance = match.Groups[17].Value;
					olaCount = match.Groups[18].Value;

					parsed = true;
				}
			}
			return parsed;
		}
		
		private string convertDuration(string Totalseconds)
		{
			var seconds = System.Convert.ToInt32(Totalseconds);
			TimeSpan time = TimeSpan.FromSeconds(seconds);

			//here backslash is must to tell that colon is
			//not the part of format, it just a character that we want in output
			string str = time.ToString(@"hh\:mm\:ss\:fff");
			
			return str;
		}
		
		public override string ToHeader()
		{
			return string.Format("GameNumber;GameIndex;Date;Time;GameLength;GameDuration;ClapAveragePower;ClapTime;ClapCount;ClapPerformance;OlaCount;FanHeat;");
		}

		public override string ToLine()
		{
			return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};", gameNumber, gameIndex, date_0, time_0, gameLength, gameDuration, clapAveragePower, clapTime, clapCount, clapPerformance, olaCount, fanHeat);
		}
		public override string ToString()
		{
			return string.Format("[LogFanGame GameNumber={0}, GameIndex={1}, Date={2}, Time={3}, GameDuration={4}, ClapAveragePower={5}, ClapTime={6}, ClapCount={7}, ClapPerformance={8}, OlaCount={9}, FanHeat={10}]", gameNumber, gameIndex, date_0, time_0, gameDuration, clapAveragePower, clapTime, clapCount, clapPerformance, olaCount, fanHeat);
		}
		
	}
}
