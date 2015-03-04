using System;
using System.Collections.Generic;

namespace Spookfiles.Koppelvlak.A
{
    public class Vehiclestate
    {
        /*
         *
         *  measurement_time	timestamp		V	O	Start tijdstip van de metingen. Voertuig data wordt batchgewijs geretourneerd voor de periode van een minuut. Dit veld beschrijft het tijdstip van de eerste meting. 
            vehicle_id	long		V	V	(Geanonimiseerd) ID van het voertuig.
            vehicle_type	vehicle_type_enum		O	O	Voertuigklasse.
            vehicle_length	integer	M	O	O	Voertuiglengte.
            feed_id 	integer		O	O	ID van de feed waarvan dit record afkomstig is indien een P1 partij het doorsturen van databronnen onderstuent.
            measurements	list<vehicle_measurement>		V	V	Voertuigmetingen zoals beschreven in de vehicle_measurement record definitie.
            No also in the tests
         */
        public DateTime measurement_time { get; set; }
        public long vehicle_id { get; set; }
        public VehicleType vehicle_type { get; set; }
        public int vehicle_length { get; set; }
        public int feed_id { get; set; }
        public List<VehicleMeasurement> measurements { get; set; }
    }
}