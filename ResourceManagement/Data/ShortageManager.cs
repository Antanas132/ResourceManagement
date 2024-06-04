using ResourceManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceManagement.Data
{
    public class ShortageManager
    {
        private readonly DataStorage _dataStorage;
        private List<Shortage> _shortages;

        public ShortageManager(DataStorage dataStorage)
        {
            _dataStorage = dataStorage;
            _shortages = _dataStorage.LoadData();
        }
        public void ClearAllShortages()
        {
            _shortages.Clear();
        }
        public void AddShortage(Shortage shortage)
        {
            // Check if a shortage with the same title and room already exists
            var existingShortage = _shortages.FirstOrDefault(s => s.Title == shortage.Title && s.Room == shortage.Room);
            if (existingShortage != null)
            {
                // If the new shortage has a higher priority, update
                if (shortage.Priority > existingShortage.Priority)
                {
                    existingShortage.Priority = shortage.Priority;
                    existingShortage.Name = shortage.Name;
                    existingShortage.Category = shortage.Category;
                    existingShortage.CreatedOn = shortage.CreatedOn;
                    existingShortage.CreatedBy = shortage.CreatedBy;
                }
                else
                {
                    Console.WriteLine("A shortage with this title and room already exists.");
                    return;
                }
            }
            else
            {
                _shortages.Add(shortage);
            }

            _dataStorage.SaveData(_shortages);
        }
       
        public void DeleteShortage(string title, string room, User user)
        {
            if (Enum.TryParse(room, true, out RoomType roomType))
            {
                var shortage = _shortages.FirstOrDefault(s => s.Title == title && s.Room == roomType);
                if (shortage == null)
                {
                    Console.WriteLine("No shortage found.");
                    return;
                }

                if (shortage.CreatedBy != user.Name && !user.IsAdmin)
                {
                    Console.WriteLine("Only the person who created the shortage or an administrator can delete it.");
                    return;
                }

                _shortages.Remove(shortage);
                _dataStorage.SaveData(_shortages);
            }
            else
            {
                Console.WriteLine("Invalid room type.");
            }
        }
        public List<Shortage> GetAllShortages(User user)
        {
            return user.IsAdmin ? _shortages : _shortages.Where(s => s.CreatedBy == user.Name).ToList();
        }
        public List<Shortage> GetAllShortages(User user, string? titleFilter = null, DateTime? startDate = null, DateTime? endDate = null, RoomType? roomFilter = null, CategoryType? categoryFilter = null)

        {
            var shortages = user.IsAdmin ? _shortages : _shortages.Where(s => s.CreatedBy == user.Name).ToList();

            if (!string.IsNullOrEmpty(titleFilter))
            {
                shortages = shortages.Where(s => s.Title.Contains(titleFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                shortages = shortages.Where(s => s.CreatedOn.Date >= startDate.Value.Date && s.CreatedOn.Date <= endDate.Value.Date).ToList();
            }

            if (roomFilter.HasValue)
            {
                shortages = shortages.Where(s => s.Room == roomFilter.Value).ToList();
            }

            if (categoryFilter.HasValue)
            {
                shortages = shortages.Where(s => s.Category == categoryFilter.Value).ToList();
            }
            return shortages;
        }
    }
}
