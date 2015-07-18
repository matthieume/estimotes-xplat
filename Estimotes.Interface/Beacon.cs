using System;


namespace Estimotes {

    public class Beacon {

		public Proximity Proximity { get; }
		public ushort? Minor { get; }
		public ushort? Major { get; }
		public string Name { get; }
		public string Identifier { get; }
		public string Uuid { get; }


		public Beacon(string uuid, string name, string identifier, Proximity proximity, ushort major, ushort minor) {
			this.Uuid = uuid;
			this.Name = name;
            this.Proximity = proximity;
            this.Major = major;
			this.Minor = minor;
        }


        // TODO: readtemperatureasync
        // TODO: getbatterylevel and battery life remaining
    }
}