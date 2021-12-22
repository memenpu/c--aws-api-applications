import React, { useState } from 'react';
import Form from 'react-bootstrap/Form';
import { Button } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import axios from 'axios';
import { css } from "@emotion/react";
import { useForm } from "react-hook-form";
import { useNavigate } from 'react-router-dom';
import ClipLoader from "react-spinners/ClipLoader";
import Moment from 'moment';
const override = css`
  display: block;
  margin: 0 auto;
  border-color: red;
`;
export default function BookRoom({ updateUser, user }) {
    const all_rooms = ['A01', 'A02', 'A03', 'A04', 'A05', 'A06', 'A07', 'A08', 'A09', 'A10', 'A11', 'A12', 'A13', 'A14', 'A15', 'A16', 'A17', 'A18', 'A19',];
    
    const today = Moment(new Date()).format("yyyy-MM-DD");
    const maxDate = Moment(new Date()).add(14, "d").format("yyyy-MM-DD");
    const [reservation, setReservation] = useState({
        roomNumber: "A01",
    reservationdate: today });
    const navigate = useNavigate();
    const { register, handleSubmit } = useForm({
        reValidateMode:"onChange",
        defaultValues:{
            roomNumber: "A01",
        reservationdate: today
    }});
    let [loading, setLoading] = useState(false);
    let [color, setColor] = useState("#ffffff");
    const [message, setMessage] = useState();
    const onSubmit = async data => {
        if(!(await isAvailable(data))){
            return;
        }
        setLoading(true);
        setMessage("Booking the room");
        setLoading(false);
        var config = {
            method: 'post',
            // url: 'http://awseb-e-n-awsebloa-vwq8ffbuatb-26814393.us-east-1.elb.amazonaws.com/api/Users/login',
            url: `http://localhost:1958/api/Reservations/book-a-room/${user.id}`,
            data: data
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
                setMessage("Room is booked by someone else.");
            });
    };
    const isAvailable = async data => {
        if(!(data.roomNumber)||!(data.reservationdate)){
            return;
        }
        setLoading(true);
        setLoading(false);
      
        let response = await axios(`http://localhost:1958/api/Reservations/check-room-availbility?roomNumber=${data.roomNumber}&reservationdate=${data.reservationdate}`);
        console.log(JSON.stringify(response.data));
        if(response.data)
            setMessage("Room is available!");
        else
            setMessage("Room unavailable on this date!");
        setLoading(false);
        return response.data;
        // axios(`http://localhost:1958/api/Reservations/check-room-availbility?roomNumber=${data.roomNumber}&reservationdate=${data.reservationdate}`)
        //     .then(function (response) {
        //         console.log(JSON.stringify(response.data));
        //         setLoading(false);
                
        //         if(response.data)
        //         setMessage("Room is available!");
        //         else
        //         setMessage("Room unavailable on this date!");
        //         return response.data;
        //     })
        //     .catch(function (error) {
        //         console.log(error);
        //         setLoading(false);
        //     });
    };


    return (
        <div className="container">
            <h5>Book A Room</h5>

            <Form onSubmit={handleSubmit(onSubmit)}>
                <Form.Group className="mb-3" controlId="formBasicEmail">
                    <Form.Label>Select a room:</Form.Label>
                    <Form.Select {...register("roomNumber")} aria-label="Default select example" 
                    onChange={async value => {
                        let newReservation = {...reservation, roomNumber: value.target.value};
                        await isAvailable(newReservation)
                        console.log(value.target.value);
                        setReservation(newReservation);
                      }}
                    required>
                        {all_rooms.map(x => <option key={x} value={x}>{x}</option>)}
                    </Form.Select>
                </Form.Group>

                <Form.Group className="mb-3" controlId="formBasicPassword">
                    <Form.Label>Select a date:</Form.Label>
                    <Form.Control type="date" {...register("reservationdate",)}
                     onChange={async value => {
                        let newReservation = {...reservation, reservationdate: value.target.value};
                        await  isAvailable(newReservation)
                        console.log(value.target.value);
                        setReservation(newReservation);
                      }}
                    min={today} max={maxDate} />
                </Form.Group>
                {message && <p>
                    {message}
                </p>}
                {loading ? <ClipLoader color={color} loading={loading} css={override} size={40} /> : <Button variant="primary" type="submit">
                    Book
                </Button>}
            </Form>
        </div>
    );
}