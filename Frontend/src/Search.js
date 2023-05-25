import React from 'react';

function Search(props) {

    const [videosData, setVideosData] = useState(null);

    useEffect(() => {
      axios.get(SEARCH_URL + "?query="+ inputValue + "&sortingCriterion=1&sortingType=1", {
        headers: { 
          'Content-Type': 'application/json',
        },
        withCredentials: true 
      })
      .then(response => {
        setVideosData(response?.data);
      })
      .catch(error => {
        console.log("error: ", error);
      });
    });
  // Component logic
  return <div>Hello, {props.query}!</div>;
}
