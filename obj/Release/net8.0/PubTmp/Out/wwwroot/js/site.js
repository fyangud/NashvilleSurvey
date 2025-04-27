// Initialize the Leaflet map
const map = L.map('map').setView([36.1627, -86.7816], 11); // Nashville coordinates

// Add OpenStreetMap base layer
/*L.tileLayer('https://{s}.tile.openstreetmap.org/transport/{z}/{x}/{y}.png', {
    attribution: '&copy; OpenStreetMap contributors'
}).addTo(map);*/

// OSM Transportation dark layer
L.tileLayer('https://tile.thunderforest.com/transport-dark/{z}/{x}/{y}.png?apikey=7b773790853b4bb5927e1624d58556ba', {
    attribution: 'Transport Map (ThunderForest)'
}).addTo(map);

// CartoDB Dark Matter base
/*L.tileLayer('https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/attributions">CARTO</a>',
    subdomains: 'abcd',
    maxZoom: 20
}).addTo(map);*/

// EPSG:2275 projection definition (for example, NAD83 / Pennsylvania South in US feet)
var epsg2275Def = '+proj=lcc +lat_1=35.25 +lat_2=36.4166666666667 +lat_0=34.3333333333333 +lon_0=-86 +x_0=600000 +y_0=0 +datum=NAD83 +units=us-ft +no_defs';

// Function to reproject a single coordinate from EPSG:2274 to EPSG:4326.
// Input: [x, y] in EPSG:2275; Output: [lng, lat] in WGS84.
function reprojectCoords(coords) {
    // Swap coordinates if they are in [northing, easting] order
    const [y, x] = coords; // Destructure as [northing, easting]
    return proj4(epsg2275Def, 'EPSG:4326', coords);
}

// Test: Philadelphia City Hall in EPSG:2274 (easting, northing)
//const testCoords = [838000, 484000];
//const testWGS84 = reprojectCoords(testCoords);
//console.log(testWGS84); 

// Reproject geometry coordinates based on the geometry type.
function reprojectGeometry(geometry) {
    var type = geometry.type;
    if (type === 'Point') {
        geometry.coordinates = reprojectCoords(geometry.coordinates);
    } else if (type === 'LineString' || type === 'MultiPoint') {
        geometry.coordinates = geometry.coordinates.map(reprojectCoords);
    } else if (type === 'Polygon' || type === 'MultiLineString') {
        geometry.coordinates = geometry.coordinates.map(function (ring) {
            return ring.map(reprojectCoords);
        });
    } else if (type === 'MultiPolygon') {
        geometry.coordinates = geometry.coordinates.map(function (polygon) {
            return polygon.map(function (ring) {
                return ring.map(reprojectCoords);
            });
        });
    }
    return geometry;
}
/*
fetch('/data/zb.geojson')
    .then(response => response.json())
    .then(data => {
        // Check if the data is a FeatureCollection or a single Feature.
        if (data.type === 'FeatureCollection') {
            data.features.forEach(function (feature) {
                feature.geometry = reprojectGeometry(feature.geometry);
            });
        } else if (data.type === 'Feature') {
            data.geometry = reprojectGeometry(data.geometry);
        }
        var geojsonLayer = L.geoJSON(data, {
            onEachFeature: function (feature, layer) {
                if (feature.properties && feature.properties.popupContent) {
                    layer.bindPopup(feature.properties.popupContent);
                }
            }
        }).addTo(map);

        map.fitBounds(geojsonLayer.getBounds());
    })
    .catch(error => console.error('Error loading GeoJSON:', error));
*/

let currentLayer;  // Track active map layer
// activate transit access visualization
switchVisualization('pub_access');
setActiveButton(document.getElementById('btn-access'));
function loadSurveyData(geojsonData, dataType = 'pub_access') {
    fetch('/api/surveyresponses')
        .then(response => response.json())
        .then(surveyData => {
            if (currentLayer) {
                map.removeLayer(currentLayer); // Clear previous layer
            }
            addGeoJsonLayer(geojsonData, surveyData, dataType);
        })
        .catch(error => console.error('Error fetching survey data:', error));
}

document.getElementById('btn-access').addEventListener('click', function () {
    switchVisualization('pub_access');
    setActiveButton(this);
});

document.getElementById('btn-commute').addEventListener('click', function () {
    switchVisualization('com_not_car');
    setActiveButton(this);
});

// Highlight active button
function setActiveButton(button) {
    document.querySelectorAll('.toggle-btn').forEach(btn => btn.classList.remove('active'));
    button.classList.add('active');
}

// Switch visualization logic, load zipcode boundary shape
function switchVisualization(dataType) {
    fetch('/data/zb.geojson')
        .then(response => response.json())
        .then(geojsonData => {
            if (geojsonData.type === 'FeatureCollection') {
                geojsonData.features.forEach(function (feature) {
                    feature.geometry = reprojectGeometry(feature.geometry);
                });
            }
            loadSurveyData(geojsonData, dataType);
        })
        .catch(error => console.error('Error loading GeoJSON:', error));
}

// Choose color based on satisfaction score
function getColor(score, dataType) {
    if (dataType === 'pub_access') {
        return score > 60 ? 'lightgreen' :
            score > 50 ? 'yellow' :
                score > 42 ? 'orange' : 'red';
    } else if (dataType === 'com_not_car') {
        return score > 0.35 ? 'green' :
            score > 0.25 ? 'lightgreen' :
                score > 0.15 ? 'yellow' : 'orange';
    }
}

function addGeoJsonLayer(geojsonData, surveyData, dataType) {
    currentLayer = L.geoJson(geojsonData, {
        style: function (feature) {
            // Use the zipcode property in the GeoJSON to find the matching survey result
            const zipcode = feature.properties.ZipCode;
            const survey = surveyData.find(item => item.zipcode === parseInt(zipcode));
            const score = survey ? survey[dataType] : null;
            //console.log(zipcode, survey);
            return {
                fillColor: survey ? getColor(score, dataType) : 'gray',
                weight: 2,
                opacity: 1,
                color: 'white',
                dashArray: '3',
                fillOpacity: 0.7
            };
        },
        onEachFeature: function (feature, layer) {
            layer.on('click', function () {
                const zipcode = feature.properties.ZipCode;
                const survey = surveyData.find(item => item.zipcode === parseInt(zipcode));
                const score = survey ? survey[dataType] : 'No data';
                layer.bindPopup(
                    `<strong>Zipcode:</strong> ${zipcode}<br>
                     <strong>Responses:</strong> ${survey['num_response']}<br>
                     <strong>${dataType === 'pub_access' ? 'Transit Access Satisfactory' : 'Commute by Transit Ratio'}:</strong> ${score}`
                ).openPopup();
            });
        }
    }).addTo(map);
}