using NUnit.Framework;
using NUnit.Framework.Legacy;
using ResourceManagement.Data;
using ResourceManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShortageManagementTests
{
    public class ShortageManagementTests
    {
        private ShortageManager _shortageManager;
        private DataStorage _dataStorage;

        [SetUp]
        public void Setup()
        {
            _dataStorage = new DataStorage();
            _shortageManager = new ShortageManager(_dataStorage);
            _shortageManager.ClearAllShortages();

        }

        [Test]
        public void AddShortage_WhenShortageDoesNotExist_ShouldAddShortage()
        {
            // Arrange
            var shortage = new Shortage
            {
                Title = "New Shortage1",
                Name = "John Doe",
                Room = RoomType.MeetingRoom,
                Category = CategoryType.Electronics,
                Priority = 1,
                CreatedOn = DateTime.Now,
                CreatedBy = "Admin"
            };

            // Act
            _shortageManager.AddShortage(shortage);

            // Assert
            var addedShortage = _shortageManager.GetAllShortages(new User { Name = "Admin", IsAdmin = true })
                .FirstOrDefault(s => s.Title == shortage.Title && s.Room == shortage.Room);
            ClassicAssert.IsNotNull(addedShortage);
            ClassicAssert.AreEqual(shortage.Name, addedShortage.Name);
            ClassicAssert.AreEqual(shortage.Category, addedShortage.Category);
            ClassicAssert.AreEqual(shortage.Priority, addedShortage.Priority);
            ClassicAssert.AreEqual(shortage.CreatedOn, addedShortage.CreatedOn);
            ClassicAssert.AreEqual(shortage.CreatedBy, addedShortage.CreatedBy);

           
          //  _shortageManager.DeleteShortage(shortage.Title, shortage.Room.ToString(), new User { Name = "Admin", IsAdmin = true });
        }

        [Test]
        public void AddShortage_WhenShortageExistsWithHigherPriority_ShouldUpdateExistingShortage()
        {
            // Arrange
            var existingShortage = new Shortage
            {
                Title = "Existing Shortage",
                Name = "John Doe",
                Room = RoomType.MeetingRoom,
                Category = CategoryType.Electronics,
                Priority = 1,
                CreatedOn = DateTime.Now,
                CreatedBy = "Admin"
            };
            _shortageManager.AddShortage(existingShortage);

            var newShortage = new Shortage
            {
                Title = "Existing Shortage",
                Name = "Jane Smith",
                Room = RoomType.MeetingRoom,
                Category = CategoryType.Food,
                Priority = 2,
                CreatedOn = DateTime.Now,
                CreatedBy = "Admin"
            };

            // Act
            _shortageManager.AddShortage(newShortage);

            // Assert
            var updatedShortage = _shortageManager.GetAllShortages(new User { Name = "Admin", IsAdmin = true })
                .FirstOrDefault(s => s.Title == existingShortage.Title && s.Room == existingShortage.Room);
            ClassicAssert.IsNotNull(updatedShortage);
            ClassicAssert.AreEqual(newShortage.Name, updatedShortage.Name);
            ClassicAssert.AreEqual(newShortage.Category, updatedShortage.Category);
            ClassicAssert.AreEqual(newShortage.Priority, updatedShortage.Priority);
            ClassicAssert.AreEqual(newShortage.CreatedOn, updatedShortage.CreatedOn);
            ClassicAssert.AreEqual(newShortage.CreatedBy, updatedShortage.CreatedBy);

           // _shortageManager.DeleteShortage(existingShortage.Title, existingShortage.Room.ToString(), new User { Name = "Admin", IsAdmin = true });
           // _shortageManager.DeleteShortage(newShortage.Title, newShortage.Room.ToString(), new User { Name = "Admin", IsAdmin = true });
        }

        [Test]
        public void DeleteShortage_WhenShortageExists_ShouldDeleteShortage()
        {
            // Arrange
            var existingShortage = new Shortage
            {
                Title = "Existing Shortage",
                Name = "John Doe",
                Room = RoomType.MeetingRoom,
                Category = CategoryType.Electronics,
                Priority = 1,
                CreatedOn = DateTime.Now,
                CreatedBy = "Admin"
            };
            _shortageManager.AddShortage(existingShortage);

            // Act
            _shortageManager.DeleteShortage(existingShortage.Title, existingShortage.Room.ToString(), new User { Name = "Admin", IsAdmin = true });

            // Assert
            var deletedShortage = _shortageManager.GetAllShortages(new User { Name = "Admin", IsAdmin = true })
                .FirstOrDefault(s => s.Title == existingShortage.Title && s.Room == existingShortage.Room);
            ClassicAssert.IsNull(deletedShortage);
        }

        [Test]
        public void DeleteShortage_WhenShortageDoesNotExist_ShouldNotDeleteShortage()
        {
            // Arrange
            var existingShortage = new Shortage
            {
                Title = "Existing Shortage",
                Name = "John Doe",
                Room = RoomType.MeetingRoom,
                Category = CategoryType.Electronics,
                Priority = 1,
                CreatedOn = DateTime.Now,
                CreatedBy = "Admin"
            };
            _shortageManager.AddShortage(existingShortage);

            // Act
            _shortageManager.DeleteShortage("Nonexistent Shortage", existingShortage.Room.ToString(), new User { Name = "Admin", IsAdmin = true });

            // Assert
            var deletedShortage = _shortageManager.GetAllShortages(new User { Name = "Admin", IsAdmin = true })
                .FirstOrDefault(s => s.Title == existingShortage.Title && s.Room == existingShortage.Room);
            ClassicAssert.IsNotNull(deletedShortage);
        }

        [Test]
        public void DeleteShortage_WhenUserIsNotAdminAndNotCreatedByUser_ShouldNotDeleteShortage()
        {
            // Arrange
            var existingShortage = new Shortage
            {
                Title = "Existing Shortage",
                Name = "John Doe",
                Room = RoomType.MeetingRoom,
                Category = CategoryType.Electronics,
                Priority = 1,
                CreatedOn = DateTime.Now,
                CreatedBy = "Admin"
            };
            _shortageManager.AddShortage(existingShortage);

            // Act
            _shortageManager.DeleteShortage(existingShortage.Title, existingShortage.Room.ToString(), new User { Name = "User", IsAdmin = false });

            // Assert
            var deletedShortage = _shortageManager.GetAllShortages(new User { Name = "Admin", IsAdmin = true })
                .FirstOrDefault(s => s.Title == existingShortage.Title && s.Room == existingShortage.Room);
            ClassicAssert.IsNotNull(deletedShortage);
        }

        [Test]
        public void GetAllShortages_WhenUserIsAdmin_ShouldReturnAllShortages()
        {
            // Arrange
            var shortage1 = new Shortage
            {
                Title = "Shortage 1",
                Name = "John Doe",
                Room = RoomType.MeetingRoom,
                Category = CategoryType.Electronics,
                Priority = 1,
                CreatedOn = DateTime.Now,
                CreatedBy = "Admin"
            };
            _shortageManager.AddShortage(shortage1);

            var shortage2 = new Shortage
            {
                Title = "Shortage 2",
                Name = "Jane Smith",
                Room = RoomType.Kitchen,
                Category = CategoryType.Food,
                Priority = 2,
                CreatedOn = DateTime.Now.AddDays(-1),
                CreatedBy = "Admin"
            };
            _shortageManager.AddShortage(shortage2);

            // Act
            var allShortages = _shortageManager.GetAllShortages(new User { Name = "Admin", IsAdmin = true });

            // Assert
            ClassicAssert.AreEqual(2, allShortages.Count);
            ClassicAssert.IsTrue(allShortages.Contains(shortage1));
            ClassicAssert.IsTrue(allShortages.Contains(shortage2));
            //_shortageManager.DeleteShortage(shortage1.Title, shortage1.Room.ToString(), new User { Name = "Admin", IsAdmin = true });
            //_shortageManager.DeleteShortage(shortage2.Title, shortage2.Room.ToString(), new User { Name = "Admin", IsAdmin = true });

        }

        [Test]
        public void GetAllShortages_WhenUserIsNotAdminAndCreatedByUser_ShouldReturnUserShortages()
        {
            // Arrange
            var userShortage1 = new Shortage
            {
                Title = "User Shortage 1",
                Name = "John Doe",
                Room = RoomType.MeetingRoom,
                Category = CategoryType.Electronics,
                Priority = 1,
                CreatedOn = DateTime.Now,
                CreatedBy = "User"
            };
            _shortageManager.AddShortage(userShortage1);

            var userShortage2 = new Shortage
            {
                Title = "User Shortage 2",
                Name = "John Doe",
                Room = RoomType.Kitchen,
                Category = CategoryType.Food,
                Priority = 2,
                CreatedOn = DateTime.Now.AddDays(-1),
                CreatedBy = "User"
            };
            _shortageManager.AddShortage(userShortage2);

            // Act
            var userShortages = _shortageManager.GetAllShortages(new User { Name = "User", IsAdmin = false });

            // Assert
            ClassicAssert.AreEqual(2, userShortages.Count);
            ClassicAssert.IsTrue(userShortages.Contains(userShortage1));
            ClassicAssert.IsTrue(userShortages.Contains(userShortage2));
        }

        [Test]
        public void GetAllShortages_WhenUserIsNotAdminAndNotCreatedByUser_ShouldReturnEmptyList()
        {
            // Arrange
            var userShortage = new Shortage
            {
                Title = "User Shortage",
                Name = "John Doe",
                Room = RoomType.MeetingRoom,
                Category = CategoryType.Electronics,
                Priority = 1,
                CreatedOn = DateTime.Now,
                CreatedBy = "User"
            };
            _shortageManager.AddShortage(userShortage);

            // Act
            var userShortages = _shortageManager.GetAllShortages(new User { Name = "Other User", IsAdmin = false });

            // Assert
            ClassicAssert.AreEqual(0, userShortages.Count);
        }

        [Test]
        public void GetAllShortages_WithFilters_ShouldReturnFilteredShortages()
        {
            // Arrange
            var shortage1 = new Shortage
            {
                Title = "Shortage 1",
                Name = "John Doe",
                Room = RoomType.MeetingRoom,
                Category = CategoryType.Electronics,
                Priority = 1,
                CreatedOn = DateTime.Now,
                CreatedBy = "Admin"
            };
            _shortageManager.AddShortage(shortage1);

            var shortage2 = new Shortage
            {
                Title = "Shortage 2",
                Name = "Jane Smith",
                Room = RoomType.Kitchen,
                Category = CategoryType.Food,
                Priority = 2,
                CreatedOn = DateTime.Now.AddDays(-1),
                CreatedBy = "Admin"
            };
            _shortageManager.AddShortage(shortage2);

            var shortage3 = new Shortage
            {
                Title = "Shortage 3",
                Name = "John Doe",
                Room = RoomType.MeetingRoom,
                Category = CategoryType.Food,
                Priority = 3,
                CreatedOn = DateTime.Now.AddDays(-2),
                CreatedBy = "Admin"
            };
            _shortageManager.AddShortage(shortage3);

            // Act
            var filteredShortages = _shortageManager.GetAllShortages(new User { Name = "Admin", IsAdmin = true }, "Shortage", DateTime.Now.AddDays(-2), DateTime.Now, RoomType.MeetingRoom, CategoryType.Food);

            // Assert
            ClassicAssert.AreEqual(1, filteredShortages.Count);
            ClassicAssert.IsTrue(filteredShortages.Contains(shortage3));
        }
    }
}
