import './App.css';
import LoginForm from './components/LoginForm';
import { Routes, Route, Link } from "react-router-dom";
import React, { useState, useEffect } from 'react';
import {ReactComponent as ReactLogo} from './logo.svg'
import SignUpForm from './components/SignUpForm';
import { Button } from 'react-bootstrap';
import Home from './components/Home';
import Room from './components/Room';
import BookRoom from './components/BookRoom';
function App() {
  
  const [user, setUser] = useState();
  useEffect(() => {
    try{ 
      setUser(JSON.parse(localStorage.getItem("user")));
    }catch(error){
      setUser(null);
    }
  },[]);
  const updateUser = (user)=>{
    localStorage.setItem('user', JSON.stringify(user));
    setUser(user);
  }
  return (
    <div className="container">
      <a href="/"><ReactLogo style={{height: 50}}/></a>
      <h3>Welcome to {user? `Reservation App ${user.firstname} ${user.lastname}`: "Reservation App"}!</h3>
     {user && <Button variant="link" style={{ textDecoration: 'none' }} onClick={()=>{
          localStorage.removeItem("user");
          setUser();
      }}>Log Out</Button>}
      <Routes>
        <Route path="/" element={<Home updateUser={updateUser} user={user}/>} />
        <Route path="/login" element={<LoginForm updateUser={updateUser} />} />
        <Route path="/signup" element={<SignUpForm />} />
        {/* <Route path="/room/:id" element={<Room  updateUser={updateUser} user={user}/>} /> */}
        <Route path="/book-room" element={<BookRoom updateUser={updateUser} user={user}/>} />
      </Routes>
    </div>
  );
}


export default App;
