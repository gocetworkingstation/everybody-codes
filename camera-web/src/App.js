import React, { useState, useEffect } from 'react';
import './App.css';
import CameraColumn from './components/CameraColumn';
import CameraMap from './components/CameraMap';
import { cameraService } from './services/cameraService';
import { sortCamerasIntoColumns, getColumnTitle, getColumnDescription } from './utils/cameraSorter';

function App() {
  const [cameras, setCameras] = useState([]);
  const [sortedCameras, setSortedCameras] = useState({});
  const [selectedCamera, setSelectedCamera] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');

  // Fetch cameras on component mount
  useEffect(() => {
    const fetchCameras = async () => {
      try {
        setLoading(true);
        setError(null);
        const cameraData = await cameraService.getAllCameras();
        setCameras(cameraData);
        setSortedCameras(sortCamerasIntoColumns(cameraData));
      } catch (err) {
        setError('Failed to load cameras. Please make sure the API is running.');
        console.error('Error loading cameras:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchCameras();
  }, []);

  // Handle search
  const handleSearch = async (term) => {
    setSearchTerm(term);
    if (term.trim() === '') {
      setSortedCameras(sortCamerasIntoColumns(cameras));
      return;
    }

    try {
      const searchResults = await cameraService.searchCameras(term);
      setSortedCameras(sortCamerasIntoColumns(searchResults));
    } catch (err) {
      setError('Search failed. Please try again.');
      console.error('Search error:', err);
    }
  };

  // Handle camera selection
  const handleCameraSelect = (camera) => {
    setSelectedCamera(camera);
  };

  // Clear search
  const clearSearch = () => {
    setSearchTerm('');
    setSortedCameras(sortCamerasIntoColumns(cameras));
  };

  if (loading) {
    return (
      <div className="app-loading">
        <div className="loading-spinner"></div>
        <h2>Loading Camera Data...</h2>
        <p>Please ensure the API is running on http://localhost:5211</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="app-error">
        <h2>❌ Error Loading Cameras</h2>
        <p>{error}</p>
        <button onClick={() => window.location.reload()}>
          🔄 Retry
        </button>
      </div>
    );
  }

  const allDisplayedCameras = Object.values(sortedCameras).flat();

  return (
    <div className="App">
      <header className="app-header">
        <div className="header-content">
          <h1>📹 Utrecht Camera Assignment</h1>
          <p className="subtitle">Camera locations sorted by divisibility rules</p>
          
          <div className="search-section">
            <div className="search-bar">
              <input
                type="text"
                placeholder="Search cameras by name..."
                value={searchTerm}
                onChange={(e) => handleSearch(e.target.value)}
                className="search-input"
              />
              {searchTerm && (
                <button onClick={clearSearch} className="clear-search">
                  ✕
                </button>
              )}
            </div>
            <div className="search-info">
              {searchTerm ? (
                <span>Showing {allDisplayedCameras.length} results for "{searchTerm}"</span>
              ) : (
                <span>Showing all {cameras.length} cameras</span>
              )}
            </div>
          </div>
        </div>
      </header>

      <main className="app-main">
        <div className="columns-section">
          <h2>📊 Camera Distribution by Numbers</h2>
          <div className="camera-columns">
            {['column1', 'column2', 'column3', 'column4'].map(columnKey => (
              <CameraColumn
                key={columnKey}
                title={getColumnTitle(columnKey)}
                description={getColumnDescription(columnKey)}
                cameras={sortedCameras[columnKey] || []}
                onCameraSelect={handleCameraSelect}
                selectedCamera={selectedCamera}
              />
            ))}
          </div>
        </div>

        <div className="map-section">
          <CameraMap
            cameras={allDisplayedCameras}
            selectedCamera={selectedCamera}
            onCameraSelect={handleCameraSelect}
          />
        </div>
      </main>

      <footer className="app-footer">
        <p>
          📈 Total Cameras: {cameras.length} | 
          🗺️ Displayed: {allDisplayedCameras.length} | 
          🎯 Selected: {selectedCamera ? `Camera ${selectedCamera.number}` : 'None'}
        </p>
      </footer>
    </div>
  );
}

export default App;
