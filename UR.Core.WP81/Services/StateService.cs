
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UR.Core.WP81.Models;

namespace UR.Core.WP81.Services
{
    public class StateService
    {
        private static StateService _context;

        public static StateService Instance
        {
            get { return _context ?? (_context = new StateService()); }
        }

        private StateService()
        {
        }

        public FileTrackHeader CurrentTrack { get; set; }
        public bool DeviceIsRegistred
        {
            get
            {
                if (string.IsNullOrEmpty(SettingsService.DeviceId))
                {
                    return false;
                }
                return true;
            }
        }
    }
}
