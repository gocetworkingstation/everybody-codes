import React from 'react';

const CameraColumn = ({ title, description, cameras, onCameraSelect, selectedCamera }) => {
  return (
    <div className="camera-column">
      <div className="column-header">
        <h3>{title}</h3>
        <p className="column-description">{description}</p>
        <span className="camera-count">{cameras.length} cameras</span>
      </div>
      
      <div className="camera-list">
        {cameras.length === 0 ? (
          <div className="no-cameras">
            <p>No cameras in this category</p>
          </div>
        ) : (
          cameras.map(camera => (
            <div 
              key={camera.number}
              className={`camera-card ${selectedCamera?.number === camera.number ? 'selected' : ''}`}
              onClick={() => onCameraSelect && onCameraSelect(camera)}
            >
              <div className="camera-header">
                <span className="camera-number">#{camera.number}</span>
                <button 
                  className="locate-button"
                  onClick={(e) => {
                    e.stopPropagation();
                    onCameraSelect && onCameraSelect(camera);
                  }}
                  title="Show on map"
                >
                  📍
                </button>
              </div>
              
              <div className="camera-details">
                <h4 className="camera-name">{camera.name}</h4>
                <div className="camera-coordinates">
                  <span>📍 {camera.latitude.toFixed(4)}, {camera.longitude.toFixed(4)}</span>
                </div>
              </div>
              
              <div className="camera-rules">
                <div className="rule-badges">
                  {camera.number % 3 === 0 && (
                    <span className="rule-badge div3">÷3</span>
                  )}
                  {camera.number % 5 === 0 && (
                    <span className="rule-badge div5">÷5</span>
                  )}
                  {camera.number % 3 !== 0 && camera.number % 5 !== 0 && (
                    <span className="rule-badge other">Other</span>
                  )}
                </div>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
};

export default CameraColumn;
