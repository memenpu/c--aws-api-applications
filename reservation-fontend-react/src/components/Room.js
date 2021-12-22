import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router';

export default function Room() {
    const { id } = useParams();
      return(
        <div>
          <h2>{id.toUpperCase()}</h2>
        </div>
      );
  }