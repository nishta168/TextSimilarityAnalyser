import dash
from dash import dcc, html, Input, Output
import plotly.express as px
import pandas as pd

# Sample data (Replace with your CSV data)
df = pd.read_csv("similarity_result.csv")
df.rename(columns={df.columns[0]: "Query"}, inplace=True)


app = dash.Dash(__name__)

app.layout = html.Div([
    html.H1("Text Similarity Analysis", style={
        "text-align": "center",
        "font-family": "Arial, sans-serif",
        "color": "#333",
        "margin-bottom": "20px"
    }),

    html.Div([
    dcc.Dropdown(
        id="reference-selector",
        options=[{"label": ref, "value": ref} for ref in df.columns[1:]],  # Exclude first column (Query)
        multi=True,
        placeholder="Select References...",
        style={
        "font-family": "Arial, sans-serif",  # Change the font
        "font-size": "16px",  # Adjust font size
        "color": "#333",  # Text color
        # "background-color": "#f8f9fa",  # Background color
        "border-radius": "8px",  # Rounded corners
        "padding": "10px",
        # "box-shadow": "0 2px 5px rgba(0,0,0,0.2)",  # Light shadow effect
        "width": "60%",  # Adjust width for better alignment
        "margin": "auto"  # Centering the dropdown
    })], style={"display": "flex", "justify-content": "center","margin-bottom": "20px"}) ,

    dcc.Graph(id="similarity-graph")
])

@app.callback(
    Output("similarity-graph", "figure"),
    Input("reference-selector", "value")
)
def update_graph(selected_refs):
    if not selected_refs:
        return px.scatter(title="Select at least one reference")

    # Filter data for selected references
    filtered_df = df[["Query"] + selected_refs]
    hover_data = ["Query"] + selected_refs  # Ensures Category appears first

    if len(selected_refs) == 1:
        fig = px.bar(filtered_df, x="Query", y=selected_refs[0], title="Bar Graph for 1 Reference")
        return fig
    elif len(selected_refs) > 3:
        melted_df = filtered_df.melt(id_vars=["Query"], var_name="Reference", value_name="Similarity")
        fig = px.bar(melted_df, x="Query", y="Similarity", color="Reference", 
                     title="Bar Graph for 3+ References", barmode="group")
        return fig
    elif len(selected_refs) == 2:
        fig = px.scatter(filtered_df, x=selected_refs[0], y=selected_refs[1], title="Scatter Plot for 2 References", hover_data=hover_data, color="Query")
    elif len(selected_refs) == 3:
        fig = px.scatter_3d(filtered_df, x=selected_refs[0], y=selected_refs[1], z=selected_refs[2], title="3D Graph for 3 References", hover_data=hover_data, color="Query")

    fig.update_traces(marker=dict(size=12, opacity=0.7))  # Set all dots to size 10

    return fig

if __name__ == "__main__":
    app.run_server(debug=True) 