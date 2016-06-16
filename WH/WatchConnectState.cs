/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 31.03.2016
 * Time: 19:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace WPF_S39_Commander.WH
{
	/// <summary>
	/// Description of WatchConnectState.
	/// </summary>
	public class WatchConnectState
	{
		#region Events
		public event ConnectionChangedEventHandler ConnectionChanged;
		protected virtual void OnConnectionChanged(CONEXSTATE oldState, CONEXSTATE state)
		{
		    if (ConnectionChanged != null)
		    	if (oldState!=state)
		        	ConnectionChanged(this, new ConnectionChangedEventArgs(oldState, state));
		}
		
		public delegate void ConnectionChangedEventHandler(object sender, ConnectionChangedEventArgs e);
		public class ConnectionChangedEventArgs : EventArgs { public ConnectionChangedEventArgs(CONEXSTATE old, CONEXSTATE state) 
			{ Old=old; State=state;} public CONEXSTATE Old {get;set;} public CONEXSTATE State{get;set;} }
		#endregion
		
		private CONEXSTATE _state = CONEXSTATE.UNKNOWN;
		public CONEXSTATE STATE { get {return _state;} set { var oldState=_state; _state = value; OnConnectionChanged(oldState, value); } }

		// .ctr
		public WatchConnectState()
		{
		}
	}
}
