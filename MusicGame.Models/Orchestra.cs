using System;
using System.Collections.Generic;
using MusicGame.Models.Activities;
using MusicGame.Models.Exceptions;

namespace MusicGame.Models
{
    public class Orchestra : IOrchestra
    {
        public int Experience { get; private set; }
        public ISet<Musician> Musicians { get; private set; }
        public ISet<Activity> UnusedActivities { get; private set; }
        public Room PracticeRoom { get; private set; }
        public string Name { get; set; }
        public int Budget { get; private set; }
        public IDictionary<int, Activity> Schedule { get; private set; }
        public int WeeklyMinutes { get; private set; }
        public int PracticeMinutes { get; private set; }
        public int RequiredPracticeForConcert { get; private set; }

        public Orchestra(string name, ISet<Musician> musicians, IDictionary<int, Activity> schedule, ISet<Activity> activities)
        {
            Experience = 0;
            Musicians = musicians;
            PracticeRoom = new Room(3, 0, "The smallest room at a very small music school - it even smells a bit");
            Name = name;
            Budget = 1000;
            Schedule = schedule;
            WeeklyMinutes = 1;
            PracticeMinutes = 0;
            UnusedActivities = activities;
            RequiredPracticeForConcert = 2;
        }

        public void BuyMusician(Musician musician)
        {
            Musicians.Add(musician);
            Budget -= musician.Price;
        }

        public void BuyPractice(Practice practice)
        {
            UnusedActivities.Add(practice);
        }

        public void BuyTrip(Trip trip)
        {
            if (Budget >= trip.Price)
            {
                Budget -= trip.Price;
                UnusedActivities.Add(trip);
            }
            else throw new NotEnoughMoneyException();

        }
        public void BuyConcert(Concert concert)
        {
            if (Budget < concert.Price) throw new NotEnoughMoneyException();
            if (Experience < concert.RequiredExperience) throw new NotEnoughExperienceException();
            Budget -= concert.Price;
            UnusedActivities.Add(concert);
        }

        public void BuyPracticeRoom(Room practiceRoom)
        {
            if (practiceRoom.Price > Budget) throw new NotEnoughMoneyException();
            if (practiceRoom.Size < Musicians.Count) throw new NotEnoughSpaceException();
            Budget -= practiceRoom.Price;
            PracticeRoom = practiceRoom;
        }

        public void UpdateSchedule(int day, Activity activity)
        {
            if (day >= Schedule.Count) throw new IndexOutOfRangeException();
            if (!UnusedActivities.Contains(activity)) throw new NullReferenceException();
            var current = Schedule[day];
            if (current != null)
                UnusedActivities.Add(current);
            UnusedActivities.Remove(activity);
            Schedule[day] = activity;
        }
    }
}