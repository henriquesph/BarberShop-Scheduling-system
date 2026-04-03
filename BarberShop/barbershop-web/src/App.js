import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './App.css';

function App() {
    const [appointments, setAppointments] = useState([]);
    const [customerName, setCustomerName] = useState('');
    const [barberName, setBarberName] = useState('Jack');

    // New States for Slot Management
    const [selectedDate, setSelectedDate] = useState('');
    const [availableSlots, setAvailableSlots] = useState([]);
    const [selectedSlot, setSelectedSlot] = useState('');

    const apiUrl = 'https://localhost:7259/api/appointments';

    useEffect(() => {
        fetchAppointments();
    }, []);

    // Whenever selectedDate changes, fetch available slots from the API
    useEffect(() => {
        if (selectedDate) {
            axios.get(`${apiUrl}/available-slots?date=${selectedDate}`)
                .then(response => {
                    setAvailableSlots(response.data);
                })
                .catch(error => console.error("Error fetching slots:", error));
        } else {
            setAvailableSlots([]);
        }
    }, [selectedDate]);

    const fetchAppointments = async () => {
        try {
            const response = await axios.get(apiUrl);
            setAppointments(response.data);
        } catch (error) {
            console.error("Error fetching data:", error);
        }
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        try {
            await axios.post(apiUrl, {
                customerName,
                barberName,
                appointmentDate: selectedSlot // We send the specific slot chosen
            });
            alert("Appointment scheduled!");
            setCustomerName('');
            setSelectedDate('');
            setSelectedSlot('');
            fetchAppointments();
        } catch (error) {
            alert("Error: " + (error.response?.data || "Could not save"));
        }
    };

    const handleCancel = async (id) => {
        if (!window.confirm("Are you sure you want to cancel this appointment?")) return;

        try {
            // Note: Use the route we fixed earlier with the /cancel suffix
            await axios.put(`${apiUrl}/${id}/cancel`);
            alert("Appointment canceled.");
            fetchAppointments(); // This refreshes the list
        } catch (error) {
            alert("Cancellation Error: " + (error.response?.data || "Error"));
        }
    };

    return (
        <div className="App" style={{ padding: '40px', fontFamily: 'sans-serif' }}>
            <h1>Barber Shop Management</h1>

            <section style={{ backgroundColor: '#f4f4f4', padding: '20px', borderRadius: '8px' }}>
                <h2>Book an Appointment</h2>
                <form onSubmit={handleCreate} style={{ display: 'flex', flexDirection: 'column', gap: '10px', maxWidth: '300px' }}>

                    <input
                        type="text"
                        placeholder="Your Name"
                        value={customerName}
                        onChange={(e) => setCustomerName(e.target.value)}
                        required
                    />

                    <select value={barberName} onChange={(e) => setBarberName(e.target.value)}>
                        <option value="Jack">Jack</option>
                        <option value="Alice">Alice</option>
                    </select>

                    {/* 1. Pick the Date first */}
                    <label>Pick a Day:</label>
                    <input
                        type="date"
                        value={selectedDate}
                        onChange={(e) => setSelectedDate(e.target.value)}
                        required
                    />

                    {/* 2. Pick the Time slot (only shows if date is selected) */}
                    <label>Available Times:</label>
                    <select
                        value={selectedSlot}
                        onChange={(e) => setSelectedSlot(e.target.value)}
                        required
                        disabled={!selectedDate}
                    >
                        <option value="">-- Choose a time --</option>
                        {availableSlots.map((slot) => (
                            <option key={slot} value={slot}>
                                {new Date(slot).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                            </option>
                        ))}
                    </select>

                    <button type="submit" disabled={!selectedSlot}>Schedule</button>
                </form>
            </section>

            <hr />

            <section>
                <h2>Schedule</h2>
                <table border="1" style={{ width: '100%', borderCollapse: 'collapse', marginTop: '20px' }}>
                    <thead>
                        <tr style={{ backgroundColor: '#eee' }}>
                            <th style={{ padding: '10px' }}>Customer</th>
                            <th>Barber</th>
                            <th>Time</th>
                            <th>Status</th>
                            <th>Actions</th> {/* Added Header */}
                        </tr>
                    </thead>
                    <tbody>
                        {appointments.length === 0 ? (
                            <tr><td colSpan="5" style={{ textAlign: 'center', padding: '10px' }}>No appointments yet.</td></tr>
                        ) : (
                            appointments.map(app => (
                                <tr key={app.id} style={{ textAlign: 'center' }}>
                                    <td style={{ padding: '10px' }}>{app.customerName}</td>
                                    <td>{app.barberName}</td>
                                    <td>{new Date(app.appointmentDate).toLocaleString()}</td>
                                    <td style={{ fontWeight: 'bold', color: app.isCanceled ? 'red' : 'green' }}>
                                        {app.isCanceled ? "❌ Canceled" : "✅ Active"}
                                    </td>
                                    <td style={{ padding: '10px' }}>
                                        {/* Only show the button if the appointment is NOT already canceled */}
                                        {!app.isCanceled && (
                                            <button
                                                onClick={() => handleCancel(app.id)}
                                                style={{ backgroundColor: '#ff4d4d', color: 'white', border: 'none', padding: '5px 10px', borderRadius: '4px', cursor: 'pointer' }}
                                            >
                                                Cancel
                                            </button>
                                        )}
                                    </td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </section>
        </div>
    );
}

export default App;