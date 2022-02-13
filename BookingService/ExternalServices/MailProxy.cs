﻿using ExploreCalifornia.BookingService.Domain;
using RestSharp;

namespace ExploreCalifornia.BookingService.ExternalServices
{
    public class MailProxy : IMailProxy
    {
        public void SendMail(Booking booking)
        {
            var client = new RestClient("http://emailservice");
            var request = new RestRequest("mail", DataFormat.Json)
                .AddJsonBody(new BookingConfirmationMail {
                    Email = booking.Email,
                    Name = booking.Name,
                    TourId = booking.TourId
                });

            client.Post(request);
        }
    }
}
