

import React from 'react';
import { Nav, ListGroup, Button, Row, Col } from "react-bootstrap";
import Moment from 'moment';

import 'bootstrap/dist/css/bootstrap.min.css';
import axios from 'axios';


export default function Home({updateUser, user}) {
    
    console.log(user);
    const deleteReseravation = async id=>{
        let config = {
            method: 'delete',
            // url: 'http://awseb-e-n-awsebloa-vwq8ffbuatb-26814393.us-east-1.elb.amazonaws.com/api/Users/login',
            url: `http://localhost:1958/api/Reservations/${id}`
          };
         let response = await axios(config);
          if(response.status == 200){
              console.log(response.data);
              response = await axios("http://localhost:1958/api/Users/"+user.email);
              if(response.status == 200){
                  updateUser(response.data);
              }
          }
    };
    
    return (
        <>
            {!user &&
                <Nav> <Nav.Item>
                    <Nav.Link href="/login">Log in</Nav.Link>
                </Nav.Item>
                    <Nav.Item>
                        <Nav.Link href="/signup">Sign Up</Nav.Link>
                    </Nav.Item>
                </Nav>
            }
            <main>
                
                {user &&
                <>
                <Button variant="link" href="/book-room">Book a room</Button>
                        <h5>My Reservations</h5> </>
                }
               { user?.reservations?.length && <ListGroup variant="flush">
                    {user?.reservations?.map(x => {
                        return <ListGroup.Item  key={x.id}>
                            <Row>
                            <Col xs={8}> Room# {x.roomNumber}, ON {Moment(x.reservationdate).format("yyyy-MM-DD")} </Col>
                            <Col> <Button  className="justify-self-end" size="sm" variant="danger" 
                            onClick={async ()=>{
                                await deleteReseravation(x.id);
                            }}
                            >Cencal</Button></Col>
                            </Row>
                        </ListGroup.Item>
                    })}
                </ListGroup>}
            </main>
        </>
    );
}