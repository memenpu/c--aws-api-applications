import React, { useState } from 'react';
import Form from 'react-bootstrap/Form';
import { Button } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import axios from 'axios';
import { css } from "@emotion/react";
import { useForm } from "react-hook-form";
import { useNavigate } from 'react-router-dom';
import ClipLoader from "react-spinners/ClipLoader";
const override = css`
  display: block;
  margin: 0 auto;
  border-color: red;
`;
export default function Login({updateUser}) {
  const navigate = useNavigate();
  const { register, handleSubmit } = useForm();
  let [loading, setLoading] = useState(false);
  let [color, setColor] = useState("#ffffff");
  const [message, setMessage] = useState();
  const onSubmit = async data => {   
    setLoading(true);
    setMessage("Loging in");
    var config = {
      method: 'post',
      // url: 'http://awseb-e-n-awsebloa-vwq8ffbuatb-26814393.us-east-1.elb.amazonaws.com/api/Users/login',
      url: 'http://localhost:1958/api/Users/login',
      data : data
    };
    axios(config)
    .then(function (response) {
      console.log(JSON.stringify(response.data));
      setLoading(false);
      updateUser(response.data);
      navigate('/');
    })
    .catch(function (error) {
      console.log(error);
      setLoading(false);
      setMessage("Email or Password is wrong!");
    });
  };  

  return (
    <div className="container">
    <h5>Log In</h5>
    
    <Form onSubmit={handleSubmit(onSubmit)}>
      <Form.Group className="mb-3" controlId="formBasicEmail">
        <Form.Label>Email address</Form.Label>
        <Form.Control type="email" placeholder="name@example.com" {...register("email", {requirej:"Please enter your email"})} />
        <Form.Text className="text-muted">
          We'll never share your email with anyone else.
        </Form.Text>
      </Form.Group>

      <Form.Group className="mb-3" controlId="formBasicPassword">
        <Form.Label>Password</Form.Label>
        <Form.Control type="password" {...register("password", {requirej:"Please enter your password"})} />
      </Form.Group>
        { message && <p>
           {message}
          </p>}
      {loading ? <ClipLoader color={color} loading={loading} css={override} size={40} /> : <Button variant="primary" type="submit">
        Login
      </Button>}
    </Form>
    </div>
  );
}