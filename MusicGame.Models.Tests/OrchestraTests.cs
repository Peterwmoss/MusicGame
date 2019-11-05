using System.Collections.Generic;
using System;
using Xunit;
using MusicGame.Models.Exceptions;
using MusicGame.Models.Activities;

namespace MusicGame.Models.Tests
{
    public class OrchestraTests
    {
        Orchestra orchestra;

        public OrchestraTests()
        {
            var dictionary = new Dictionary<int, Activity>();
            for (var i = 0; i < 7; i++)
                dictionary.Add(i, null);
            orchestra = new Orchestra("Test", new HashSet<Musician>(), dictionary, new HashSet<Activity>());
        }

        [Fact]
        public void BuyMusician_given_musician_adds_musician_to_orchestra()
        {
            var musician = new Musician("Musician", Instrument.Basoon, "Loves tests", 3, 100);

            orchestra.BuyMusician(musician);

            Assert.True(orchestra.Musicians.Contains(musician));
        }

        [Fact]
        public void BuyMusician_given_musician_removes_price_from_budget()
        {
            var musician = new Musician("Musician", Instrument.Basoon, "Loves tests", 3, 100);

            orchestra.BuyMusician(musician);

            Assert.Equal(900, orchestra.Budget);
        }

        [Fact]
        public void BuyPractice_added_to_unusedActivities()
        {
            var practice = new Practice(1);

            orchestra.BuyPractice(practice);

            Assert.Contains(practice, orchestra.UnusedActivities);
        }

        [Fact]
        public void BuyConcert_added_to_unusedActivities()
        {
            var concert = new Concert(1, 0, "TestLocation", 0, 0, 0);

            orchestra.BuyConcert(concert);

            Assert.Contains(concert, orchestra.UnusedActivities);
        }

        [Fact]
        public void BuyTrip_added_to_unusedActivities()
        {
            var trip = new Trip(1, 0, "TestLocation", 0);

            orchestra.BuyTrip(trip);

            Assert.Contains(trip, orchestra.UnusedActivities);
        }

        [Theory]
        [InlineData(100, 900)]
        [InlineData(50, 950)]
        [InlineData(1000, 0)]
        [InlineData(500, 500)]
        public void BuyTrip_price_removed_from_budget(int price, int newBudget)
        {
            var trip = new Trip(1, price, "TestLocation", 0);

            orchestra.BuyTrip(trip);

            Assert.Equal(newBudget, orchestra.Budget);
        }

        [Theory]
        [InlineData(100, 900)]
        [InlineData(50, 950)]
        [InlineData(1000, 0)]
        [InlineData(500, 500)]
        public void BuyConcert_price_removed_from_budget(int price, int newBudget)
        {
            var concert = new Concert(1, price, "TestLocation", 0, 0, 0);

            orchestra.BuyConcert(concert);

            Assert.Equal(newBudget, orchestra.Budget);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(1000)]
        public void BuyConcert_not_enough_experience_throws_exception(int requiredExperience)
        {
            var concert = new Concert(1, 0, "TestLocation", 0, 0, requiredExperience);

            Assert.Throws<NotEnoughExperienceException>(() => orchestra.BuyConcert(concert));
        }

        [Theory]
        [InlineData(1001)]
        [InlineData(2000)]
        [InlineData(5000)]
        public void BuyConcert_not_enough_money_throws_exception(int price)
        {
            var concert = new Concert(1, price, "TestLocation", 0, 0, 0);

            Assert.Throws<NotEnoughMoneyException>(() => orchestra.BuyConcert(concert));
        }

        [Theory]
        [InlineData(1001)]
        [InlineData(2000)]
        [InlineData(5000)]
        public void BuyTrip_not_enough_money_throws_exception(int price)
        {
            var trip = new Trip(1, price, "TestLocation", 0);

            Assert.Throws<NotEnoughMoneyException>(() => orchestra.BuyTrip(trip));
        }

        [Fact]
        public void UpdateSchedule_given_day_and_activity_adds_to_schedule()
        {
            var activity = new Practice(1);
            orchestra.UnusedActivities.Add(activity);

            orchestra.UpdateSchedule(0, activity);

            Assert.Equal(activity, orchestra.Schedule[0]);
        }

        [Fact]
        public void UpdateSchedule_given_day_and_activity_overrides_current_schedule()
        {
            var activity1 = new Practice(1);
            var activity2 = new Practice(2);
            orchestra.UnusedActivities.Add(activity1);
            orchestra.UnusedActivities.Add(activity2);

            orchestra.UpdateSchedule(0, activity1);
            orchestra.UpdateSchedule(0, activity2);

            Assert.Equal(activity2, orchestra.Schedule[0]);
        }

        [Fact]
        public void UpdateSchedule_given_day_and_activity_returns_overriden_activities_to_unused()
        {
            var activity1 = new Practice(1);
            var activity2 = new Practice(2);
            orchestra.UnusedActivities.Add(activity1);
            orchestra.UnusedActivities.Add(activity2);

            orchestra.UpdateSchedule(0, activity1);
            orchestra.UpdateSchedule(0, activity2);

            Assert.Contains(activity1, orchestra.UnusedActivities);
        }

        [Fact]
        public void UpdateSchedule_given_day_and_activity_removes_from_unused()
        {
            var activity = new Practice(1);
            orchestra.UnusedActivities.Add(activity);

            orchestra.UpdateSchedule(0, activity);

            Assert.DoesNotContain(activity, orchestra.UnusedActivities);
        }

        [Fact]
        public void UpdateSchedule_given_day_and_activity_not_in_range_throws_exeption()
        {
            var activity = new Practice(1);
            orchestra.UnusedActivities.Add(activity);

            Assert.Throws<IndexOutOfRangeException>(() => orchestra.UpdateSchedule(7, activity));
        }

        [Fact]
        public void UpdateSchedule_given_day_and_activity_not_in_unused_throws_exeption()
        {
            var activity = new Practice(1);

            Assert.Throws<NullReferenceException>(() => orchestra.UpdateSchedule(0, activity));
        }

        [Theory]
        [InlineData(1, 999)]
        [InlineData(100, 900)]
        [InlineData(500, 500)]
        [InlineData(1000, 0)]
        public void BuyPracticeRoom_removes_money(int price, int expected)
        {
            var room = new Room(1, price, "TestLocation");

            orchestra.BuyPracticeRoom(room);

            Assert.Equal(expected, orchestra.Budget);
        }

        [Fact]
        public void BuyPracticeRoom_not_enough_money_throws_exception()
        {
            var room = new Room(1, 1001, "TestLocation");

            Assert.Throws<NotEnoughMoneyException>(() => orchestra.BuyPracticeRoom(room));
        }

        [Fact]
        public void BuyPracticeRoom_not_big_enough_throws_exception()
        {
            var musician1 = new Musician("Peter", Instrument.Basoon, "Loves tests", 1, 0);
            var musician2 = new Musician("Mie", Instrument.Oboe, "Testing", 1, 0);
            orchestra.BuyMusician(musician1);
            orchestra.BuyMusician(musician2);

            var room = new Room(1, 1, "TestLocation");

            Assert.Throws<NotEnoughSpaceException>(() => orchestra.BuyPracticeRoom(room));
        }

        [Fact]
        public void BuyPracticeRoom_updates_practice_room()
        {
            var room = new Room(1, 1, "TestLocation");

            orchestra.BuyPracticeRoom(room);

            Assert.Equal(room, orchestra.PracticeRoom);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(60)]
        [InlineData(120)]
        public void RunScheduledWeek_updates_PracticeMinutes(int minutes)
        {
            orchestra.Schedule[1] = new Practice(minutes);

            orchestra.RunScheduledWeek();

            Assert.Equal(minutes, orchestra.PracticeMinutes);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(2000)]
        [InlineData(5000)]
        public void RunScheduledWeek_with_concert_updates_Experience(int experience)
        {
            orchestra.Schedule[1] = new Concert(0, 0, "Test", experience, 0, 0);

            orchestra.RunScheduledWeek();

            Assert.Equal(experience, orchestra.Experience);
        }

        [Theory]
        [InlineData(100, 200, 1100)]
        [InlineData(50, 200, 1150)]
        [InlineData(0, 200, 1200)]
        [InlineData(100, 0, 900)]
        [InlineData(100, 50, 1050)]
        public void RunScheduledWeek_with_concert_updates_Budget(int price, int revenue, int expected)
        {
            orchestra.Schedule[1] = new Concert(0, price, "Test", 0, revenue, 0);

            orchestra.RunScheduledWeek();

            Assert.Equal(expected, orchestra.Budget);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(99)]
        [InlineData(200)]
        [InlineData(1000)]
        public void RunScheduledWeek_with_trip_updates_experience(int experience)
        {
            var trip = new Trip(1, 0, "Test", experience);
            orchestra.Schedule[1] = trip;
            orchestra.RunScheduledWeek();

            Assert.Equal(experience, orchestra.Experience);
        }

        [Fact]
        public void RunScheduledWeek_clears_schedule()
        {
            orchestra.Schedule[1] = new Trip(1, 1, "Test", 1);
            orchestra.Schedule[2] = new Practice(1);

            orchestra.RunScheduledWeek();

            Assert.Empty(orchestra.Schedule);
        }

        [Theory]
        [InlineData(5, 3)]
        [InlineData(10, 8)]
        [InlineData(100, 98)]
        [InlineData(1000, 998)]
        public void RunScheduledWeek_with_concert_subtracts_requiredPracticeForConcert(int duration, int newValue)
        {
            
            orchestra.Schedule[1] = new Practice(duration);

            orchestra.RunScheduledWeek();

            orchestra.Schedule[0] = new Concert(0, 0, "Test", 0, 0, 0);
           
            Assert.Equal(newValue, orchestra.PracticeMinutes);

        }
    }
}
