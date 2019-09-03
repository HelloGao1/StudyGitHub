using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Partner.Data.Integration.Models;

namespace Partner.Data.Integration.Hubs
{
    public class SensorDataHub:Hub
    {
        /// <summary>
     ///  Join the specified patient sensor data channel.
     /// </summary>
     /// <param name="patientId"></param>
        public void JoinPatient(string patientId)
        {
            Groups.Add(Context.ConnectionId, patientId);
        }

        /// <summary>
        ///  Leave the specified patient sensor data channel.
        ///  Call before close the connection.
        /// </summary>
        /// <param name="patientId"></param>
        public void LeavePatient(string patientId)
        {
            Groups.Remove(Context.ConnectionId, patientId);
        }

        public void SendSensorData(string patientId, SensorData sensorData)
        {
            Clients.Group(patientId).broadCastSensorData(sensorData);
        }
    }

}