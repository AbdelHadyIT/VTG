function initFields() {    
    document.getElementById("Name").value = "";
    document.getElementById("Address").value = "";
    document.getElementById("Lat").value = "";
    document.getElementById("Lng").value = "";
    document.getElementById("ReservationPrice").value = "";
    document.getElementById("PhoneNumber").value = "";
    document.getElementById("Details").value = "";
    document.getElementById("Rating").value = "";
}
var map;
var infoWindow;
var service;
var markers = [];
function initMap() {
    var myLatlng = new google.maps.LatLng(34.80208544142896, 38.996811152237456);
    var myOptions = { zoom: 7, center: myLatlng }
    map = new google.maps.Map(document.getElementById("mapp"), myOptions);
                
    google.maps.event.addListener(map, 'click', function (event) {
        service = new google.maps.places.PlacesService(map);
        infoWindow = new google.maps.InfoWindow();
        var request = {
            location: event.latLng,                        
            radius: 500,
            //keyword: "all"
            types: [document.getElementById("regiontype").value]
        };
        markers.forEach(function (marker) {
            marker.setMap(null);
        });
        service.radarSearch(request, callback);

    });
    var marker
    map.addListener('rightclick', function (e) {
        initFields()
        if (marker != null) {
            marker.setMap(null);
        }
        map.setCenter(e.latLng);
        marker = new google.maps.Marker({
            map: map,
        });
        var infowindow = new google.maps.InfoWindow();
        marker.setPosition(e.latLng);
        document.getElementById("Lat").value = e.latLng.lat();
        document.getElementById("Lng").value = e.latLng.lng();
        infowindow.setContent("Latitude: " + e.latLng.lat() +"<br/>" + "Longitude: " + e.latLng.lng());
        infowindow.open(map, marker);
    });
    //---------------------------------------------------------------
    //for search box
    var input = document.getElementById('pac-input');
    var searchBox = new google.maps.places.SearchBox(input);
    map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

    // Bias the SearchBox results towards current map's viewport.
    map.addListener('bounds_changed', function() {
        searchBox.setBounds(map.getBounds());
    });

    var Markers = [];
    // Listen for the event fired when the user selects a prediction and retrieve
    // more details for that place.
    searchBox.addListener('places_changed', function() {
        var places = searchBox.getPlaces();

        if (places.length == 0) {
            return;
        }

        // Clear out the old markers.
        Markers.forEach(function(marker) {
            marker.setMap(null);
        });
        Markers = [];

        // For each place, get the icon, name and location.
        var bounds = new google.maps.LatLngBounds();
        places.forEach(function(place) {
            if (!place.geometry) {
                console.log("Returned place contains no geometry");
                return;
            }
            var icon = {
                url: place.icon,
                size: new google.maps.Size(71, 71),
                origin: new google.maps.Point(0, 0),
                anchor: new google.maps.Point(17, 34),
                scaledSize: new google.maps.Size(25, 25)
            };

            // Create a marker for each place.
            Markers.push(new google.maps.Marker({
                map: map,
                icon: icon,
                title: place.name,
                position: place.geometry.location
            }));

            if (place.geometry.viewport) {
                // Only geocodes have viewport.
                bounds.union(place.geometry.viewport);
            } else {
                bounds.extend(place.geometry.location);
            }
        });
        map.fitBounds(bounds);
    });
    //for search box
    //---------------------------------------------------------------
}//end initMap()


function callback(results, status) {
    if (status !== google.maps.places.PlacesServiceStatus.OK) {
        console.error(status);
        return;
    }
    //markers = results;
    for (var i = 0, result; result = results[i]; i++) {
        addMarker(result);
        //markers[i] = result;
    }
}

//for add marker to the map and add listner to its
function addMarker(place) {
    var marker = new google.maps.Marker({
        map: map,
        position: place.geometry.location,
        icon: {
            url: 'http://maps.gstatic.com/mapfiles/circle.png',
            anchor: new google.maps.Point(10, 10),
            scaledSize: new google.maps.Size(10, 17)
        }
    });

    markers.push(marker);

    google.maps.event.addListener(marker, 'click', function (e) {
        initFields();
        service.getDetails(place, function (result, status) {
            if (status !== google.maps.places.PlacesServiceStatus.OK) {
                console.error(status);
                return;
            }
            document.getElementById("Name").value = result.name;
            document.getElementById("Address").value = result.formatted_address;
            document.getElementById("Lat").value = e.latLng.lat();
            document.getElementById("Lng").value = e.latLng.lng();
            if (result.international_phone_number != null) {
                document.getElementById("PhoneNumber").value = result.international_phone_number;
            }
            else {
                document.getElementById("PhoneNumber").value = "";
            }
            //document.getElementById("Type").value = e.type;
            document.getElementById("Rating").value = result.rating;
            infoWindow.setContent(result.name + "<br>" + result.formatted_address + "<br>" + result.international_phone_number);
            infoWindow.open(map, marker);
        });
    });
}
