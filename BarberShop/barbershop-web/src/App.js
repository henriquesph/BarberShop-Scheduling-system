import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './App.css';

function App() {
    const [appointments, setAppointments] = useState([]);
    const [customerName, setCustomerName] = useState('');
    const [barberName, setBarberName] = useState('Jack');
    const [appointmentDate, setAppointmentDate] = useState('');

    const apiUrl = 'https://localhost:7259/api/appointments';

    useEffect(() => {
        fetchAppointments();
    }, []);

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
                appointmentDate
            });
            alert("Appointment scheduled successfully!");
            setCustomerName('');
            setAppointmentDate('');
            fetchAppointments();
        } catch (error) {
            alert("Scheduling Error: " + (error.response?.data || "Invalid request"));
        }
    };

    const handleCancel = async (id) => {
        if (!window.confirm("Are you sure?")) return;
        try {
            await axios.put(`${apiUrl}/${id}/cancel`);
            alert("Appointment canceled.");
            fetchAppointments();
        } catch (error) {
            alert("Cancellation Error: " + (error.response?.data || "Error"));
        }
    };

    return (
        <div className="App" style={{ padding: '40px', fontFamily: 'sans-serif' }}>
            <header>
                <h1>Barber Shop Management</h1>
            </header>

            <section style={{ margin: '20px 0', padding: '20px', backgroundColor: '#f9f9f9' }}>
                <h2>Book Appointment</h2>
                <form onSubmit={handleCreate}>
                    <input
                        type="text"
                        placeholder="Customer Name"
                        value={customerName}
                        onChange={(e) => setCustomerName(e.target.value)}
                        required
                    />
                    <select value={barberName} onChange={(e) => setBarberName(e.target.value)}>
                        <option value="Jack">Jack</option>
                        <option value="Alice">Alice</option>
                    </select>
                    <input
                        type="datetime-local"
                        value={appointmentDate}
                        onChange={(e) => setAppointmentDate(e.target.value)}
                        required
                    />
                    <button type="submit">Schedule</button>
                </form>
            </section>

            <section>
                <h2>Upcoming Appointments</h2>
                <table border="1" style={{ width: '100%', textAlign: 'left', borderCollapse: 'collapse' }}>
                    <thead>
                        <tr>
                            <th>Customer</th>
                            <th>Barber</th>
                            <th>Date & Time</th>
                            <th>Status</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {appointments.map(app => (
                            <tr key={app.id}>
                                <td>{app.customerName}</td>
                                <td>{app.barberName}</td>
                                <td>{new Date(app.appointmentDate).toLocaleString()}</td>
                                <td>{app.isCanceled ? "Canceled" : "Active"}</td>
                                <td>
                                    {!app.isCanceled && (
                                        <button onClick={() => handleCancel(app.id)}>Cancel</button>
                                    )}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </section>
        </div>
    );
}

export default App;