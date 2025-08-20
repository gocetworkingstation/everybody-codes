import axios from 'axios';

const API_BASE_URL = 'http://localhost:5211/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const cameraService = {
  /**
   * Get all cameras
   * @returns {Promise<Array>} Array of camera objects
   */
  async getAllCameras() {
    try {
      const response = await api.get('/cameras');
      return response.data;
    } catch (error) {
      console.error('Error fetching cameras:', error);
      throw new Error('Failed to fetch cameras');
    }
  },

  /**
   * Search cameras by name
   * @param {string} name - Search term
   * @returns {Promise<Array>} Array of matching camera objects
   */
  async searchCameras(name) {
    try {
      const response = await api.get(`/cameras/search?name=${encodeURIComponent(name)}`);
      return response.data;
    } catch (error) {
      console.error('Error searching cameras:', error);
      throw new Error('Failed to search cameras');
    }
  },

  /**
   * Get camera by number
   * @param {number} number - Camera number
   * @returns {Promise<Object>} Camera object
   */
  async getCameraByNumber(number) {
    try {
      const response = await api.get(`/cameras/${number}`);
      return response.data;
    } catch (error) {
      console.error('Error fetching camera:', error);
      throw new Error('Failed to fetch camera');
    }
  },
};

export default cameraService;
