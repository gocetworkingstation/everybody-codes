import React, { useEffect, useRef } from 'react';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';

// Fix for default markers in React
delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: require('leaflet/dist/images/marker-icon-2x.png'),
  iconUrl: require('leaflet/dist/images/marker-icon.png'),
  shadowUrl: require('leaflet/dist/images/marker-shadow.png'),
});

const CameraMap = ({ cameras, selectedCamera, onCameraSelect }) => {
  const mapRef = useRef(null);
  const mapInstanceRef = useRef(null);
  const markersRef = useRef([]);

  useEffect(() => {
    // Initialize map
    if (!mapInstanceRef.current && mapRef.current) {
      mapInstanceRef.current = L.map(mapRef.current).setView([52.0914, 5.1115], 14);

      // Add OpenStreetMap tiles
      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap contributors',
        maxZoom: 19,
      }).addTo(mapInstanceRef.current);
    }

    return () => {
      // Cleanup on unmount
      if (mapInstanceRef.current) {
        mapInstanceRef.current.remove();
        mapInstanceRef.current = null;
      }
    };
  }, []);

  useEffect(() => {
    // Update markers when cameras change
    if (mapInstanceRef.current && cameras) {
      // Clear existing markers
      markersRef.current.forEach(marker => {
        mapInstanceRef.current.removeLayer(marker);
      });
      markersRef.current = [];

      // Add new markers
      cameras.forEach(camera => {
        const marker = L.marker([camera.latitude, camera.longitude])
          .addTo(mapInstanceRef.current)
          .bindPopup(`
            <div>
              <h3>Camera ${camera.number}</h3>
              <p><strong>Location:</strong> ${camera.name}</p>
              <p><strong>Coordinates:</strong> ${camera.latitude.toFixed(6)}, ${camera.longitude.toFixed(6)}</p>
            </div>
          `)
          .on('click', () => {
            if (onCameraSelect) {
              onCameraSelect(camera);
            }
          });

        markersRef.current.push(marker);
      });

      // Fit map to show all markers if we have cameras
      if (cameras.length > 0) {
        const group = new L.featureGroup(markersRef.current);
        mapInstanceRef.current.fitBounds(group.getBounds().pad(0.1));
      }
    }
  }, [cameras, onCameraSelect]);

  useEffect(() => {
    // Highlight selected camera
    if (mapInstanceRef.current && selectedCamera) {
      const selectedMarker = markersRef.current.find(marker => {
        const lat = marker.getLatLng().lat;
        const lng = marker.getLatLng().lng;
        return Math.abs(lat - selectedCamera.latitude) < 0.000001 && 
               Math.abs(lng - selectedCamera.longitude) < 0.000001;
      });

      if (selectedMarker) {
        selectedMarker.openPopup();
        mapInstanceRef.current.setView(selectedMarker.getLatLng(), 16);
      }
    }
  }, [selectedCamera]);

  return (
    <div className="camera-map">
      <div className="map-header">
        <h2>📍 Camera Locations</h2>
        <p className="map-info">
          {cameras ? `${cameras.length} cameras` : 'Loading cameras...'}
        </p>
      </div>
      <div 
        ref={mapRef} 
        className="map-container"
        style={{ 
          height: '500px', 
          width: '100%',
          border: '2px solid #ddd',
          borderRadius: '8px'
        }}
      />
    </div>
  );
};

export default CameraMap;
