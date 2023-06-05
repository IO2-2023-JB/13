import React, { createContext, useState, useEffect } from 'react';
import axios from './axios';

export const ConstrainsContext = createContext();

export const ConstrainsProvider = ({ children }) => {
  const [baseURL, setBaseApiUrl] = useState('https://io2.azurewebsites.net/api/');

  useEffect(() => {
    const savedApiUrl = localStorage.getItem('apiUrl');
    if (savedApiUrl) {
      setBaseApiUrl(savedApiUrl);
      axios.defaults.baseURL = savedApiUrl;
    } else {
      setBaseApiUrl('https://io2.azurewebsites.net/api/');
    }
  }, []);

  const setApiUrl = (url) => {
    setBaseApiUrl(url);
    localStorage.setItem('apiUrl', url);
  };

  return (
    <ConstrainsContext.Provider value={{ baseURL, setApiUrl }}>
      {children}
    </ConstrainsContext.Provider>
  );
};
