import React, { createContext, useState } from 'react';

export const ConstrainsContext = createContext();

export const ConstrainsProvider = ({ children }) => {
  const [baseURL, setBaseApiUrl] = useState('https://io2.azurewebsites.net/api/');

  const setApiUrl = (url) => {
    setBaseApiUrl(url);
  };

  return (
    <ConstrainsContext.Provider value={{ baseURL, setApiUrl }}>
      {children}
    </ConstrainsContext.Provider>
  );
};
